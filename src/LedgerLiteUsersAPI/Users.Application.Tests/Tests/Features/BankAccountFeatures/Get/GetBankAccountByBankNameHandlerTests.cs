using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.BankAccountFeatures.Get.GetByBankName;
using Users.Domain.Entities;
using Users.Domain.Entities.Enums;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.BankAccountFeatures.Get
{
    public class GetBankAccountByBankNameHandlerTests : IDisposable
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly AppDbContext _databaseContext;
        private readonly GetBankAccountByBankNameHandler _bankAccountHandler;

        // Constructor:
        public GetBankAccountByBankNameHandlerTests()
        {
            _bankAccountRepository = Substitute.For<IBankAccountRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);
            _bankAccountHandler = new GetBankAccountByBankNameHandler(_bankAccountRepository);
        }

        // Methods:
        private static GetBankAccountByBankNameQuery CreateValidQuery(string bankName) => new(bankName);

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
        public async Task Handle_ShouldReturnSuccess_WhenBankHasNoAccounts()
        {
            // Arrange:
            const string bankName = "Nubank";

            GetBankAccountByBankNameQuery query = CreateValidQuery(bankName);

            _bankAccountRepository
                .GetBankAccountByBankNameAsync(query.BankName, Arg.Any<CancellationToken>())
                .Returns(Enumerable.Empty<BankAccount>());

            // Act:
            Result<GetBankAccountByBankNameResponse> result = await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.BanksAccountsList.Should().NotBeNull();
            result.Value.BanksAccountsList.Should().BeEmpty();

            await _bankAccountRepository
                .Received(1)
                .GetBankAccountByBankNameAsync(bankName, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenBankHasAccounts()
        {
            // Arrange:
            const string bankName = "Nubank";

            List<BankAccount> bankAccounts =
            [
                CreateValidBankAccount(Guid.NewGuid(), Guid.NewGuid(), bankName),
                CreateValidBankAccount(Guid.NewGuid(), Guid.NewGuid(), bankName)
            ];

            GetBankAccountByBankNameQuery query = CreateValidQuery(bankName);

            _bankAccountRepository
                .GetBankAccountByBankNameAsync(query.BankName, Arg.Any<CancellationToken>())
                .Returns(bankAccounts);

            // Act:
            Result<GetBankAccountByBankNameResponse> result = await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.BanksAccountsList.Should().HaveCount(2);
            result.Value.BanksAccountsList.Should().BeEquivalentTo(bankAccounts);

            await _bankAccountRepository
                .Received(1)
                .GetBankAccountByBankNameAsync(bankName, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnlyOnce_WhenBankNameIsValid()
        {
            // Arrange:
            const string bankName = "Nubank";

            List<BankAccount> bankAccounts =
            [
                CreateValidBankAccount(Guid.NewGuid(), Guid.NewGuid(), bankName)
            ];

            GetBankAccountByBankNameQuery query = CreateValidQuery(bankName);

            _bankAccountRepository
                .GetBankAccountByBankNameAsync(query.BankName, Arg.Any<CancellationToken>())
                .Returns(bankAccounts);

            // Act:
            await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            await _bankAccountRepository
                .Received(1)
                .GetBankAccountByBankNameAsync(bankName, Arg.Any<CancellationToken>());
        }
    }
}