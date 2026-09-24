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

        // Constructor:
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

        // Methods:
        private static UpdateUserCommand CreateValidCommand(Guid userId)
        {
            return new UpdateUserCommand(
                Id: userId,
                Name: "Pedro",
                Surname: "Henrique",
                Birthdate: DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                Email: "pedro@email.com",
                Phone: "11999999999",
                IsActive: true
            );
        }

        private static User CreateValidUser(Guid userId)
        {
            User user = new(
                name: "João",
                surname: "Silva",
                birthdate: DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
                email: "joao@email.com",
                phone: "11988888888",
                isActive: false
            );

            typeof(User)
                .GetProperty(nameof(User.Id))!
                .SetValue(user, userId);

            return user;
        }

        public void Dispose()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Dispose();

            GC.SuppressFinalize(this);
        }

        // Tests:
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

            _userRepository.DidNotReceive().Update(Arg.Any<User>());
        }

        [Fact]
        public async Task Handle_ShouldUpdateUser_WhenCommandIsValid()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();

            UpdateUserCommand command = CreateValidCommand(userId);
            User user = CreateValidUser(userId);

            _userRepository
                .GetUserByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            User? updatedUser = null;

            _userRepository
                .When(repository => repository.Update(Arg.Any<User>()))
                .Do(call => updatedUser = call.Arg<User>());

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

            updatedUser.Should().NotBeNull();
            updatedUser!.Name.Should().Be(command.Name);
            updatedUser.Surname.Should().Be(command.Surname);
            updatedUser.Birthdate.Should().Be(command.Birthdate);
            updatedUser.Email.Should().Be(command.Email);
            updatedUser.Phone.Should().Be(command.Phone);
            updatedUser.IsActive.Should().Be(command.IsActive);

            _userRepository.Received(1).Update(Arg.Any<User>());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnlyOnce_WhenCommandIsValid()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();

            UpdateUserCommand command = CreateValidCommand(userId);
            User user = CreateValidUser(userId);

            _userRepository
                .GetUserByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act:
            await _userHandler.Handle(command, CancellationToken.None);

            // Assert:
            await _userRepository
                .Received(1)
                .GetUserByIdAsync(command.Id, Arg.Any<CancellationToken>());

            _userRepository
                .Received(1)
                .Update(Arg.Any<User>());
        }
    }
}