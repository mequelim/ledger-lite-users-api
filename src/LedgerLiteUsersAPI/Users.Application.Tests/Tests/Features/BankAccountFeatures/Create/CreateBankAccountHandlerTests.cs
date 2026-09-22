using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.BankAccountFeatures.Create;
using Users.Domain.Entities;
using Users.Domain.Entities.Enums;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.BankAccountFeatures.Create
{
    public class CreateBankAccountHandlerTests : IDisposable
    {
        private readonly IUserRepository _userRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly AppDbContext _databaseContext;
        private readonly CreateBankAccountHandler _bankAccountHandler;

        // Constructor:
        public CreateBankAccountHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _bankAccountRepository = Substitute.For<IBankAccountRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);

            _bankAccountHandler = new CreateBankAccountHandler(
                _userRepository,
                _bankAccountRepository,
                _databaseContext
            );
        }

        // Methods:
        private static CreateBankAccountCommand CreateValidCommand()
        {
            return new CreateBankAccountCommand(
                UserId: Guid.NewGuid(),
                BankName: "Nubank",
                Holder: "Pedro Henrique",
                AccountNumber: "12345678-9",
                Agency: "0001",
                BankAccountType: BankAccountType.Business
            );
        }

        private static User CreateValidUser(Guid userId)
        {
            User user = new(
                name: "Pedro",
                surname: "Henrique",
                birthdate: new DateOnly(1998, 5, 10),
                email: "pedro@email.com",
                phone: "11999999999",
                isActive: true
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
            CreateBankAccountCommand command = CreateValidCommand();

            _userRepository
                .GetUserByIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act:
            Result<CreateBankAccountResponse> result = await _bankAccountHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found!");

            _bankAccountRepository.DidNotReceive().Create(Arg.Any<BankAccount>());
        }

        [Fact]
        public async Task Handle_ShouldCreateBankAccount_WhenCommandIsValid()
        {
            // Arrange:
            CreateBankAccountCommand command = CreateValidCommand();

            User user = CreateValidUser(command.UserId);

            _userRepository
                .GetUserByIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);

            BankAccount? createdBankAccount = null;

            _bankAccountRepository
                .When(repository => repository.Create(Arg.Any<BankAccount>()))
                .Do(call => createdBankAccount = call.Arg<BankAccount>());

            // Act:
            Result<CreateBankAccountResponse> result = await _bankAccountHandler.Handle(command, CancellationToken.None);

            // Assert:
            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Id.Should().NotBe(Guid.Empty);
            result.Value.UserId.Should().Be(command.UserId);
            result.Value.BankName.Should().Be(command.BankName);
            result.Value.Holder.Should().Be(command.Holder);
            result.Value.AccountNumber.Should().Be(command.AccountNumber);
            result.Value.Agency.Should().Be(command.Agency);
            result.Value.BankAccountType.Should().Be(command.BankAccountType);

            createdBankAccount.Should().NotBeNull();

            _bankAccountRepository.Received(1).Create(Arg.Any<BankAccount>());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenBankAccountNumberIsInvalid()
        {
            // Arrange:
            CreateBankAccountCommand command = CreateValidCommand() with
            {
                AccountNumber = "123456789"
            };

            User user = CreateValidUser(command.UserId);

            _userRepository
                .GetUserByIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act:
            Result<CreateBankAccountResponse> result = await _bankAccountHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrWhiteSpace();

            _bankAccountRepository.DidNotReceive().Create(Arg.Any<BankAccount>());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenBankAccountAgencyIsInvalid()
        {
            // Arrange:
            CreateBankAccountCommand command = CreateValidCommand() with
            {
                Agency = "123"
            };

            User user = CreateValidUser(command.UserId);

            _userRepository
                .GetUserByIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act:
            Result<CreateBankAccountResponse> result = await _bankAccountHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrWhiteSpace();

            _bankAccountRepository.DidNotReceive().Create(Arg.Any<BankAccount>());
        }
    }
}