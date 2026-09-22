using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.BankAccountFeatures.Get.GetById;
using Users.Domain.Entities;
using Users.Domain.Entities.Enums;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.BankAccountFeatures.Get
{
    public class GetBankAccountByIdHandlerTests : IDisposable
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly AppDbContext _databaseContext;
        private readonly GetBankAccountByIdHandler _bankAccountHandler;

        // Constructor:
        public GetBankAccountByIdHandlerTests()
        {
            _bankAccountRepository = Substitute.For<IBankAccountRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);
            _bankAccountHandler = new GetBankAccountByIdHandler(_bankAccountRepository);
        }

        // Methods:
        private static GetBankAccountByIdQuery CreateValidQuery(Guid accountId) => new(accountId);

        private static BankAccount CreateValidBankAccount(Guid accountId, Guid userId)
        {
            BankAccount bankAccount = new(
                bankName: "Nubank",
                holder: "Pedro Henrique",
                accountNumber: "12345678-9",
                agency: "0001",
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
        public async Task Handle_ShouldReturnFailure_WhenIdIsEmpty()
        {
            // Arrange:
            GetBankAccountByIdQuery query = new(Guid.Empty);

            // Act:
            Result<GetBankAccountByIdResponse> result = await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("The bank account id cannot be null!");

            await _bankAccountRepository
                .DidNotReceive()
                .GetBankAccountByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenBankAccountDoesNotExist()
        {
            // Arrange:
            Guid accountId = Guid.NewGuid();

            GetBankAccountByIdQuery query = CreateValidQuery(accountId);

            _bankAccountRepository
                .GetBankAccountByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns((BankAccount?)null);

            // Act:
            Result<GetBankAccountByIdResponse> result = await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Bank account not found!");

            await _bankAccountRepository
                .Received(1)
                .GetBankAccountByIdAsync(accountId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenBankAccountExists()
        {
            // Arrange:
            Guid accountId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            GetBankAccountByIdQuery query = CreateValidQuery(accountId);

            BankAccount bankAccount = CreateValidBankAccount(accountId, userId);

            _bankAccountRepository
                .GetBankAccountByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns(bankAccount);

            // Act:
            Result<GetBankAccountByIdResponse> result = await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(accountId);

            await _bankAccountRepository
                .Received(1)
                .GetBankAccountByIdAsync(accountId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnlyOnce_WhenBankAccountExists()
        {
            // Arrange:
            Guid accountId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            GetBankAccountByIdQuery query = CreateValidQuery(accountId);

            BankAccount bankAccount = CreateValidBankAccount(accountId, userId);

            _bankAccountRepository
                .GetBankAccountByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns(bankAccount);

            // Act:
            await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            await _bankAccountRepository
                .Received(1)
                .GetBankAccountByIdAsync(accountId, Arg.Any<CancellationToken>());
        }
    }
}