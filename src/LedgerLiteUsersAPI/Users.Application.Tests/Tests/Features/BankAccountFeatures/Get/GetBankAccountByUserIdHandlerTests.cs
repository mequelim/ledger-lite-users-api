using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.BankAccountFeatures.Get.GetByUserId;
using Users.Domain.Entities;
using Users.Domain.Entities.Enums;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.BankAccountFeatures.Get
{
    public class GetBankAccountByUserIdHandlerTests : IDisposable
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly AppDbContext _databaseContext;
        private readonly GetBankAccountByUserIdHandler _bankAccountHandler;

        // Constructor:
        public GetBankAccountByUserIdHandlerTests()
        {
            _bankAccountRepository = Substitute.For<IBankAccountRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);
            _bankAccountHandler = new GetBankAccountByUserIdHandler(_bankAccountRepository);
        }

        // Methods:
        private static GetBankAccountByUserIdQuery CreateValidQuery(Guid userId) => new(userId);

        private static BankAccount CreateValidBankAccount(Guid accountId, Guid userId, string bankName)
        {
            BankAccount bankAccount = new(
                bankName: bankName,
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
        public async Task Handle_ShouldReturnFailure_WhenUserIdIsEmpty()
        {
            // Arrange:
            GetBankAccountByUserIdQuery query = new(Guid.Empty);

            // Act:
            Result<GetBankAccountByUserIdResponse> result = await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found!");

            await _bankAccountRepository
                .DidNotReceive()
                .GetBankAccountByUserIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserHasNoBankAccounts()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();

            GetBankAccountByUserIdQuery query = CreateValidQuery(userId);

            _bankAccountRepository
                .GetBankAccountByUserIdAsync(query.UserId, Arg.Any<CancellationToken>())
                .Returns(Enumerable.Empty<BankAccount>());

            // Act:
            Result<GetBankAccountByUserIdResponse> result = await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.BanksAccountsList.Should().NotBeNull();
            result.Value.BanksAccountsList.Should().BeEmpty();

            await _bankAccountRepository
                .Received(1)
                .GetBankAccountByUserIdAsync(userId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserHasBankAccounts()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();

            GetBankAccountByUserIdQuery query = CreateValidQuery(userId);

            List<BankAccount> bankAccounts =
            [
                CreateValidBankAccount(Guid.NewGuid(), userId, "Nubank"),
                CreateValidBankAccount(Guid.NewGuid(), userId, "Inter")
            ];

            _bankAccountRepository
                .GetBankAccountByUserIdAsync(query.UserId, Arg.Any<CancellationToken>())
                .Returns(bankAccounts);

            // Act:
            Result<GetBankAccountByUserIdResponse> result = await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.BanksAccountsList.Should().HaveCount(2);
            result.Value.BanksAccountsList.Should().BeEquivalentTo(bankAccounts);

            await _bankAccountRepository
                .Received(1)
                .GetBankAccountByUserIdAsync(userId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnlyOnce_WhenUserIdIsValid()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();

            GetBankAccountByUserIdQuery query = CreateValidQuery(userId);

            List<BankAccount> bankAccounts =
            [
                CreateValidBankAccount(Guid.NewGuid(), userId, "Nubank")
            ];

            _bankAccountRepository
                .GetBankAccountByUserIdAsync(query.UserId, Arg.Any<CancellationToken>())
                .Returns(bankAccounts);

            // Act:
            await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            await _bankAccountRepository
                .Received(1)
                .GetBankAccountByUserIdAsync(userId, Arg.Any<CancellationToken>());
        }
    }
}