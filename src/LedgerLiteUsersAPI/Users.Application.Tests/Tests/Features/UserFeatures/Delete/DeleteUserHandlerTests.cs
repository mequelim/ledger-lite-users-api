using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.UserFeatures.Delete;
using Users.Domain.Entities;
using Users.Domain.Exceptions.UserExceptions;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Delete
{
    public class DeleteUserHandlerTests : IDisposable
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _databaseContext;
        private readonly DeleteUserHandler _userHandler;

        public DeleteUserHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);
            _userHandler = new DeleteUserHandler(
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

            DeleteUserCommand command = new(userId);

            _userRepository
                .DeleteAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromException<User>(
                    new UserNotFoundException(nameof(User.Id), command.Id)
                ));

            // Act:
            Result<DeleteUserResponse> result = await _userHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be(
                new UserNotFoundException(nameof(User.Id), command.Id).Message
            );

            await _userRepository
                .Received(1)
                .DeleteAsync(command.Id, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldDeleteUser_WhenCommandIsValid()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();

            DeleteUserCommand command = new(userId);

            User user = new(
                name: "Pedro",
                surname: "Henrique",
                birthdate: DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                email: "pedro@email.com",
                phone: "11999999999",
                isActive: true
            );

            _userRepository
                .DeleteAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(user));

            // Act:
            Result<DeleteUserResponse> result = await _userHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(command.Id);

            await _userRepository
                .Received(1)
                .DeleteAsync(command.Id, Arg.Any<CancellationToken>());
        }
    }
}