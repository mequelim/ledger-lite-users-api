using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.UserFeatures.Update;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Update
{
    public class UpdateUserHandlerTests : IDisposable
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _databaseContext;
        private readonly UpdateUserHandler _userHandler;

        public UpdateUserHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);
            _userHandler = new UpdateUserHandler(
                _userRepository,
                _databaseContext
            );
        }

        public void Dispose()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Dispose();

            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();

            UpdateUserCommand command = CreateValidCommand(userId);

            _userRepository
                .GetUserByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act:
            Result<UpdateUserResponse> result = await _userHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found!");

            _userRepository
                .DidNotReceive()
                .Update(Arg.Any<User>());
        }

        [Fact]
        public async Task Handle_ShouldUpdateUser_WhenUserExists()
        {
            // Arrange:
            User user = new(
                "Pedro",
                "Silva",
                DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                "old@email.com",
                "11999999999",
                true
            );

            UpdateUserCommand command = CreateValidCommand(user.Id);

            _userRepository
                .GetUserByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            _userRepository
                .Update(Arg.Any<User>())
                .Returns(user);

            // Act:
            Result<UpdateUserResponse> result = await _userHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(command.Id);
            result.Value.Name.Should().Be(command.Name);
            result.Value.Surname.Should().Be(command.Surname);
            result.Value.Birthdate.Should().Be(command.Birthdate);
            result.Value.Email.Should().Be(command.Email);
            result.Value.Phone.Should().Be(command.Phone);
            result.Value.IsActive.Should().Be(command.IsActive);

            _userRepository
                .Received(1)
                .Update(user);
        }

        private static UpdateUserCommand CreateValidCommand(Guid userId)
        {
            return new UpdateUserCommand(
                userId,
                "Updated Name",
                "Updated Surname",
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
                "updated@email.com",
                "11888888888",
                false
            );
        }
    }
}