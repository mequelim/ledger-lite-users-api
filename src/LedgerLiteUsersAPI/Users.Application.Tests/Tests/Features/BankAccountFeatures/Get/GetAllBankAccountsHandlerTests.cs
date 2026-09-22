using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.BankAccountFeatures.Get.GetAll;
using Users.Domain.Entities;
using Users.Domain.Entities.Enums;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.BankAccountFeatures.Get
{
    public class GetAllBankAccountsHandlerTests : IDisposable
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly AppDbContext _databaseContext;
        private readonly GetAllBankAccountsHandler _bankAccountHandler;

        // Constructor:
        public GetAllBankAccountsHandlerTests()
        {
            _bankAccountRepository = Substitute.For<IBankAccountRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);
            _bankAccountHandler = new GetAllBankAccountsHandler(_bankAccountRepository);
        }

        // Methods:
        private static List<BankAccount> CreateBankAccounts()
        {
            Guid userId = Guid.NewGuid();

            BankAccount firstBankAccount = new(
                bankName: "Nubank",
                holder: "Pedro Henrique",
                accountNumber: "12345678-9",
                agency: "0001",
                bankAccountType: BankAccountType.Checking,
                userId: userId
            );

            BankAccount secondBankAccount = new(
                bankName: "Inter",
                holder: "Pedro Henrique",
                accountNumber: "98765432-1",
                agency: "0002",
                bankAccountType: BankAccountType.Business,
                userId: userId
            );

            return
            [
                firstBankAccount,
                secondBankAccount
            ];
        }

        public void Dispose()
        {
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Dispose();

            GC.SuppressFinalize(this);
        }

        // Tests:
        [Fact]
        public async Task Handle_ShouldReturnAllBankAccounts_WhenBankAccountsExist()
        {
            // Arrange:
            IEnumerable<BankAccount> bankAccounts = CreateBankAccounts();

            IEnumerable<BankAccount> returnThis = bankAccounts.ToList();
            _bankAccountRepository
                .GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(returnThis);

            GetAllBankAccountsQuery query = new();

            // Act:
            Result<GetAllBankAccountsResponse> result = await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.BankAccountsList.Should().HaveCount(2);
            result.Value.BankAccountsList.Should().BeEquivalentTo(returnThis);

            await _bankAccountRepository
                .Received(1)
                .GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenBankAccountsListIsEmpty()
        {
            // Arrange:
            IEnumerable<BankAccount> bankAccounts = Enumerable.Empty<BankAccount>();

            _bankAccountRepository
                .GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(bankAccounts);

            GetAllBankAccountsQuery query = new();

            // Act:
            Result<GetAllBankAccountsResponse> result = await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.BankAccountsList.Should().BeEmpty();

            await _bankAccountRepository
                .Received(1)
                .GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnlyOnce()
        {
            // Arrange:
            IEnumerable<BankAccount> bankAccounts = CreateBankAccounts();

            _bankAccountRepository
                .GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(bankAccounts);

            GetAllBankAccountsQuery query = new();

            // Act:
            await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            await _bankAccountRepository
                .Received(1)
                .GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnBankAccountsInRepositoryOrder()
        {
            // Arrange:
            List<BankAccount> bankAccounts = [.. CreateBankAccounts()];

            _bankAccountRepository
                .GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(bankAccounts);

            GetAllBankAccountsQuery query = new();

            // Act:
            Result<GetAllBankAccountsResponse> result = await _bankAccountHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.Value.BankAccountsList.ElementAt(0).BankName.Should().Be("Nubank");
            result.Value.BankAccountsList.ElementAt(1).BankName.Should().Be("Inter");
        }
    }
}