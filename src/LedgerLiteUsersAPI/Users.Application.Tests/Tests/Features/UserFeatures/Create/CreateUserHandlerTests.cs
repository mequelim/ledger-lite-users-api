using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.UserFeatures.Create;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Create
{
    public class CreateUserHandlerTests : IDisposable
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _databaseContext;
        private readonly CreateUserHandler _userHandler;

        // Constructor:
        public CreateUserHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);
            _userHandler = new CreateUserHandler(
                _userRepository,
                _databaseContext
            );
        }

        // Methods:
        private static CreateUserCommand CreateValidCommand()
        {
            return new CreateUserCommand(
                Name: "Pedro",
                Surname: "Henrique",
                Birthdate: DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                Email: "pedro@email.com",
                Phone: "11999999999",
                IsActive: true
            );
        }

        public void Dispose()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Dispose();

            GC.SuppressFinalize(this);
        }

        // Tests:
        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenEmailAlreadyExists()
        {
            // Arrange:
            CreateUserCommand command = CreateValidCommand();
            User existingUser = new(
                command.Name,
                command.Surname,
                command.Birthdate,
                command.Email,
                command.Phone,
                command.IsActive
            );

            _userRepository
                .GetUserByEmailAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns(existingUser);

            // Act:
            Result<CreateUserResponse> result = await _userHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User e-mail already exists!");

            _userRepository.DidNotReceive().Create(Arg.Any<User>());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenPhoneAlreadyExists()
        {
            // Arrange:
            CreateUserCommand command = CreateValidCommand();
            User existingUser = new(
                command.Name,
                command.Surname,
                command.Birthdate,
                command.Email,
                command.Phone,
                command.IsActive
            );

            _userRepository
                .GetUserByEmailAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            _userRepository
                .GetUserByPhoneAsync(command.Phone, Arg.Any<CancellationToken>())
                .Returns(existingUser);

            // Act:
            Result<CreateUserResponse> result = await _userHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User phone already exists!");

            _userRepository.DidNotReceive().Create(Arg.Any<User>());
        }

        [Fact]
        public async Task Handle_ShouldCreateUser_WhenCommandIsValid()
        {
            // Arrange:
            CreateUserCommand command = CreateValidCommand();

            _userRepository
                .GetUserByEmailAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            _userRepository
                .GetUserByPhoneAsync(command.Phone, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            User? createdUser = null;

            _userRepository
                .When((repository) => repository.Create(Arg.Any<User>()))
                .Do((call) => createdUser = call.Arg<User>());

            // Act:
            Result<CreateUserResponse> result = await _userHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Id.Should().NotBe(Guid.Empty);
            result.Value.Name.Should().Be(command.Name);
            result.Value.Surname.Should().Be(command.Surname);
            result.Value.Birthdate.Should().Be(command.Birthdate);
            result.Value.Email.Should().Be(command.Email);
            result.Value.Phone.Should().Be(command.Phone);
            result.Value.IsActive.Should().Be(command.IsActive);

            createdUser.Should().NotBeNull();

            _userRepository.Received(1).Create(Arg.Any<User>());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenEmailIsInvalid()
        {
            // Arrange:
            CreateUserCommand command = CreateValidCommand() with
            {
                Email = "email-invalido"
            };

            _userRepository
                .GetUserByEmailAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            _userRepository
                .GetUserByPhoneAsync(command.Phone, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act:
            Result<CreateUserResponse> result = await _userHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrWhiteSpace();

            _userRepository.DidNotReceive().Create(Arg.Any<User>());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenPhoneIsInvalid()
        {
            // Arrange:
            CreateUserCommand command = CreateValidCommand() with
            {
                Phone = "12345"
            };

            _userRepository
                .GetUserByEmailAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            _userRepository
                .GetUserByPhoneAsync(command.Phone, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act:
            Result<CreateUserResponse> result = await _userHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrWhiteSpace();

            _userRepository.DidNotReceive().Create(Arg.Any<User>());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserAgeIsInvalid()
        {
            // Arrange:
            CreateUserCommand command = CreateValidCommand() with
            {
                Birthdate = DateOnly.FromDateTime(DateTime.Today.AddYears(-10))
            };

            _userRepository
                .GetUserByEmailAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            _userRepository
                .GetUserByPhoneAsync(command.Phone, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act:
            Result<CreateUserResponse> result = await _userHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrWhiteSpace();

            _userRepository.DidNotReceive().Create(Arg.Any<User>());
        }
    }
}