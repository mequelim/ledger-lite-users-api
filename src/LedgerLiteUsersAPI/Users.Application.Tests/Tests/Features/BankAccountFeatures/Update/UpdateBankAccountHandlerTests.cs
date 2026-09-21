using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.BankAccountFeatures.Update;
using Users.Domain.Entities;
using Users.Domain.Entities.Enums;
using Users.Domain.Exceptions.BankAccount;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.BankAccountFeatures.Update
{
    public class UpdateBankAccountHandlerTests : IDisposable
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly AppDbContext _databaseContext;
        private readonly UpdateBankAccountHandler _bankAccountHandler;

        // Constructor:
        public UpdateBankAccountHandlerTests()
        {
            _bankAccountRepository = Substitute.For<IBankAccountRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);

            _bankAccountHandler = new UpdateBankAccountHandler(
                _bankAccountRepository,
                _databaseContext
            );
        }

        // Methods:
        private static UpdateBankAccountCommand CreateValidCommand(Guid accountId, Guid userId)
        {
            return new UpdateBankAccountCommand(
                Id: accountId,
                BankName: "Nubank",
                Holder: "Pedro Henrique",
                AccountNumber: "12345678-9",
                Agency: "0001",
                BankAccountType: BankAccountType.Business
            );
        }

        private static BankAccount CreateValidBankAccount(Guid accountId, Guid userId)
        {
            BankAccount bankAccount = new(
                bankName: "Inter",
                holder: "Old Holder",
                accountNumber: "99999999-9",
                agency: "1234",
                bankAccountType: BankAccountType.Checking,
                userId: userId
            );

            typeof(BankAccount)
                .GetProperty(nameof(BankAccount.Id))!
                .SetValue(bankAccount, accountId);

            return bankAccount;
        }

        public void Dispose()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Dispose();

            GC.SuppressFinalize(this);
        }

        // Tests:
        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenBankAccountDoesNotExist()
        {
            // Arrange:
            Guid accountId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            UpdateBankAccountCommand command = CreateValidCommand(accountId, userId);

            _bankAccountRepository
                .GetBankAccountByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns((BankAccount?)null);

            // Act:
            Result<UpdateBankAccountResponse> result = await _bankAccountHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Bank account not found!");

            _bankAccountRepository.DidNotReceive().Update(Arg.Any<BankAccount>());
        }

        [Fact]
        public async Task Handle_ShouldUpdateBankAccount_WhenCommandIsValid()
        {
            // Arrange:
            Guid accountId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            UpdateBankAccountCommand command = CreateValidCommand(accountId, userId);

            BankAccount bankAccount = CreateValidBankAccount(accountId, userId);

            _bankAccountRepository
                .GetBankAccountByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(bankAccount);

            _bankAccountRepository
                .Update(Arg.Any<BankAccount>())
                .Returns(call => call.Arg<BankAccount>());

            // Act:
            Result<UpdateBankAccountResponse> result = await _bankAccountHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(accountId);
            result.Value.UserId.Should().Be(userId);
            result.Value.BankName.Should().Be(command.BankName);
            result.Value.Holder.Should().Be(command.Holder);
            result.Value.AccountNumber.Should().Be(command.AccountNumber);
            result.Value.Agency.Should().Be(command.Agency);
            result.Value.BankAccountType.Should().Be(command.BankAccountType);

            bankAccount.BankName.Should().Be(command.BankName);
            bankAccount.Holder.Should().Be(command.Holder);
            bankAccount.AccountNumber.Should().Be(command.AccountNumber);
            bankAccount.Agency.Should().Be(command.Agency);
            bankAccount.BankAccountType.Should().Be(command.BankAccountType);

            _bankAccountRepository.Received(1).Update(bankAccount);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryThrowsInvalidAccountNumberException()
        {
            // Arrange:
            Guid accountId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            UpdateBankAccountCommand command = CreateValidCommand(accountId, userId);
            BankAccount bankAccount = CreateValidBankAccount(accountId, userId);

            _bankAccountRepository
                .GetBankAccountByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(bankAccount);

            _bankAccountRepository
                .When(repository => repository.Update(Arg.Any<BankAccount>()))
                .Do(_ => throw new InvalidAccountNumberException(command.AccountNumber));

            // Act:
            Result<UpdateBankAccountResponse> result = await _bankAccountHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryThrowsInvalidAgencyException()
        {
            // Arrange:
            Guid accountId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            UpdateBankAccountCommand command = CreateValidCommand(accountId, userId);
            BankAccount bankAccount = CreateValidBankAccount(accountId, userId);

            _bankAccountRepository
                .GetBankAccountByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(bankAccount);

            _bankAccountRepository
                .When(repository => repository.Update(Arg.Any<BankAccount>()))
                .Do(_ => throw new InvalidAgencyException(command.Agency));

            // Act:
            Result<UpdateBankAccountResponse> result = await _bankAccountHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryThrowsInvalidBankNameException()
        {
            // Arrange:
            Guid accountId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            UpdateBankAccountCommand command = CreateValidCommand(accountId, userId);
            BankAccount bankAccount = CreateValidBankAccount(accountId, userId);

            _bankAccountRepository
                .GetBankAccountByIdAsync(command.Id, Arg.Any<CancellationToken>())
                .Returns(bankAccount);

            _bankAccountRepository
                .When(repository => repository.Update(Arg.Any<BankAccount>()))
                .Do(_ => throw new InvalidBankNameException(command.BankName));

            // Act:
            Result<UpdateBankAccountResponse> result = await _bankAccountHandler.Handle(command, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }
    }
}