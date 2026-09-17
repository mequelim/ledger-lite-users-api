using Users.Domain.Entities;
using Users.Domain.Exceptions.BankAccount;
using Users.Persistence.Database;
using Users.Persistence.Repositories;
using Users.Persistence.Tests.Fixtures;
using Users.Persistence.Tests.Mocks.Domain;

namespace Users.Persistence.Tests.Tests.Repositories
{
    public class BankAccountRepositoryTests
    {
        private readonly AppDbContext _dbContext;
        private readonly BankAccountRepository _repository;
        private readonly CancellationToken _cancellationToken = CancellationToken.None;

        // Constructor:
        public BankAccountRepositoryTests()
        {
            _dbContext = PersistenceTestContext.CreateInMemoryDbContext();
            _repository = new BankAccountRepository(_dbContext);
        }

        // Methods:
        private BankAccount GenerateCompleteBankAccount() => BankAccountFactory.CreateDefault();

        private BankAccount GenerateBankAccountWithoutHolder() => BankAccountFactory.CreateWithoutHolder();

        private async Task<User> PersistUserAsync()
        {
            User user = new UserBuilder().Build();
            user.BankAccounts.Clear();

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        // Tests:
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllBankAccounts_WhenMultipleAccountsExist()
        {
            // Arrange:
            BankAccount account01 = GenerateCompleteBankAccount();
            BankAccount account02 = GenerateBankAccountWithoutHolder();

            await _dbContext.BankAccounts.AddRangeAsync(account01, account02);
            await _dbContext.SaveChangesAsync();

            // Act:
            IEnumerable<BankAccount> result = await _repository.GetAllAsync(_cancellationToken);

            // Assert:
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetBankAccountByIdAsync_ShouldReturnABankAccount_WhenBankAccountExist()
        {
            // Arrange:
            User user = await PersistUserAsync();
            BankAccount account = GenerateCompleteBankAccount();

            await _dbContext.BankAccounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            // Act:
            BankAccount result = await _repository.GetBankAccountByIdAsync(account.Id, _cancellationToken);

            // Assert:
            Assert.NotNull(result);
            Assert.NotNull(result.AccountNumber);
            Assert.NotEmpty(result.AccountNumber);
            Assert.NotNull(result.Agency);
            Assert.NotEmpty(result.Agency);
            Assert.NotNull(result.BankName);
            Assert.NotEmpty(result.BankName);
            Assert.NotNull(result.Holder);
        }

        [Fact]
        public async Task GetBankAccountByIdAsync_ShouldThrowArgumentException_WhenBankAccountIdIsEmpty()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetBankAccountByIdAsync(Guid.Empty, _cancellationToken));
        }

        [Fact]
        public async Task GetBankAccountByIdAsync_ShouldThrowBankAccountNotFoundException_WhenBankAccountDoesNotExist()
        {
            await Assert.ThrowsAsync<BankAccountNotFoundException>(() => _repository.GetBankAccountByIdAsync(Guid.NewGuid(), _cancellationToken));
        }

        [Fact]
        public async Task GetBankAccountByUserIdAsync_ShouldReturnAllAccounts_WhenUserHasMultipleAccounts()
        {
            // Arrange:
            User user = await PersistUserAsync();
            User otherUser = await PersistUserAsync();

            BankAccount account01 = GenerateCompleteBankAccount();
            account01.UserId = user.Id;

            BankAccount account02 = GenerateBankAccountWithoutHolder();
            account02.UserId = user.Id;

            BankAccount otherAccount = GenerateCompleteBankAccount();
            otherAccount.UserId = otherUser.Id;

            await _dbContext.BankAccounts.AddRangeAsync(account01, account02, otherAccount);
            await _dbContext.SaveChangesAsync();

            // Act:
            IEnumerable<BankAccount?> result = await _repository.GetBankAccountByUserIdAsync(user.Id, _cancellationToken);
            IEnumerable<BankAccount> bankAccounts = result as BankAccount[] ?? [.. result!];

            // Assert:
            Assert.Equal(2, bankAccounts.Count());
            Assert.All(bankAccounts, (bankAccount) => Assert.Equal(user.Id, bankAccount.UserId));
        }

        [Fact]
        public async Task GetBankAccountByUserIdAsync_ShouldReturnEmpty_WhenUserHasNoAccounts()
        {
            // Arrange:
            User user = await PersistUserAsync();

            // Act:
            IEnumerable<BankAccount?> result = await _repository.GetBankAccountByUserIdAsync(user.Id, _cancellationToken);

            // Assert:
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetBankAccountByUserIdAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
        {
            // Assert, Act: & Arrange:
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetBankAccountByUserIdAsync(Guid.Empty, _cancellationToken));
        }

        [Fact]
        public async Task GetBankAccountByUserNameAsync_ShouldReturnAccounts_WhenNameMatches()
        {
            // Arrange:
            User user = await PersistUserAsync();

            BankAccount account = GenerateCompleteBankAccount();
            account.User = user;
            account.UserId = user.Id;
            account.Holder = $"{user.Name} {user.Surname}";

            await _dbContext.BankAccounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            // Act:
            IEnumerable<BankAccount> result = await _repository.GetBankAccountByUserNameAsync("Pedro", _cancellationToken);

            // Assert
            Assert.Contains(result, b => b.Id == account.Id);
        }

        [Fact]
        public async Task GetBankAccountByUserNameAsync_ShouldReturnAccounts_WhenSurnameMatches()
        {
            // Arrange:
            User user = await PersistUserAsync();

            BankAccount account = GenerateCompleteBankAccount();
            account.User = user;
            account.UserId = user.Id;
            account.Holder = $"{user.Name} {user.Surname}";

            await _dbContext.BankAccounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            // Act:
            IEnumerable<BankAccount> result = await _repository.GetBankAccountByUserNameAsync("Mequelim", _cancellationToken);

            // Assert:
            Assert.Contains(result, b => b.Id == account.Id);
        }

        [Fact]
        public async Task GetBankAccountByUserNameAsync_ShouldReturnAccounts_WhenFullNameMatches()
        {
            // Arrange:
            User user = await PersistUserAsync();

            BankAccount account = GenerateCompleteBankAccount();
            account.User = user;
            account.UserId = user.Id;
            account.Holder = $"{user.Name} {user.Surname}";

            await _dbContext.BankAccounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            // Act:
            IEnumerable<BankAccount> result = await _repository.GetBankAccountByUserNameAsync("Pedro Mequelim", _cancellationToken);

            // Assert:
            Assert.Contains(result, b => b.Id == account.Id);
        }

        [Fact]
        public async Task GetBankAccountByUserNameAsync_ShouldReturnEmpty_WhenNoUserMatches()
        {
            User user = await PersistUserAsync();
            BankAccount account = GenerateCompleteBankAccount();

            await _dbContext.BankAccounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            IEnumerable<BankAccount> result = await _repository.GetBankAccountByUserNameAsync("Fernanda", _cancellationToken);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetBankAccountByUserNameAsync_ShouldThrowArgumentException_WhenUserNameIsNullOrWhitespace(string? userName)
        {
            // Assert, Act: & Arrange:
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetBankAccountByUserNameAsync(userName!, _cancellationToken));
        }

        [Fact]
        public async Task GetBankAccountByBankNameAsync_ShouldReturnAccounts_WhenBankNameMatches()
        {
            // Arrange:
            BankAccount account = GenerateCompleteBankAccount();
            BankAccount otherAccount = GenerateCompleteBankAccount();

            otherAccount.BankName = "Itaú";

            await _dbContext.BankAccounts.AddRangeAsync(account, otherAccount);
            await _dbContext.SaveChangesAsync();

            // Act:
            IEnumerable<BankAccount> result = await _repository.GetBankAccountByBankNameAsync("Santander", _cancellationToken);
            IEnumerable<BankAccount> bankAccounts = result as BankAccount[] ?? [.. result];

            // Assert:
            Assert.Single(bankAccounts);
            Assert.Equal(account.Id, bankAccounts.Single().Id);
        }

        [Fact]
        public async Task GetBankAccountByBankNameAsync_ShouldReturnEmpty_WhenNoAccountMatches()
        {
            // Arrange:
            BankAccount account = GenerateBankAccountWithoutHolder();
            account.BankName = "Nubank";

            // Act:
            await _dbContext.BankAccounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            IEnumerable<BankAccount> result = await _repository.GetBankAccountByBankNameAsync("Santander", _cancellationToken);

            // Assert:
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetBankAccountByBankNameAsync_ShouldThrowInvalidBankNameException_WhenBankNameIsNullOrWhitespace(string? bankName)
        {
            // Assert, Act: & Arrange:
            await Assert.ThrowsAsync<InvalidBankNameException>(() => _repository.GetBankAccountByBankNameAsync(bankName!, _cancellationToken));
        }

        [Fact]
        public async Task CreateAsync_ShouldPersistAndReturnBankAccount_WhenDataIsValid()
        {
            // Arrange
            BankAccount account = GenerateCompleteBankAccount();
            BankAccount result = _repository.Create(account);

            // Act:
            BankAccount? persisted = await _dbContext.BankAccounts.FindAsync(account.Id);

            _dbContext.ChangeTracker.Clear();

            // Assert:
            Assert.Equal(account.Id, result.Id);
            Assert.NotNull(persisted);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndReturnBankAccount_WhenDataIsValid()
        {
            // Arrange:
            BankAccount account = GenerateCompleteBankAccount();

            await _dbContext.BankAccounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            account.BankName = "Updated bank";

            // Act:
            BankAccount result = _repository.Update(account);
            BankAccount? persisted = await _dbContext.BankAccounts.FindAsync(account.Id);

            // Assert:
            Assert.Equal("Updated bank", result.BankName);
            Assert.Equal("Updated bank", persisted!.BankName);
        }

        [Fact]
        public void UpdateAsync_ShouldThrowInvalidBankAccountAccountNumberException_WhenAccountNumberIsInvalid()
        {
            // Arrange & Act:
            BankAccount account = GenerateCompleteBankAccount();
            account.AccountNumber = string.Empty;

            // Assert:
            Assert.Throws<InvalidBankAccountNumberException>(() => _repository.Update(account));
        }

        [Fact]
        public void UpdateAsync_ShouldThrowInvalidBankAccountAgencyException_WhenAgencyIsInvalid()
        {
            // Arrange & Act:
            BankAccount account = GenerateCompleteBankAccount();
            account.Agency = string.Empty;

            // Assert:
            Assert.Throws<InvalidBankAccountAgencyException>(() => _repository.Update(account));
        }

        [Fact]
        public void UpdateAsync_ShouldThrowInvalidBankNameException_WhenBankNameIsNullOrWhitespace()
        {
            // Arrange & Act:
            BankAccount account = GenerateCompleteBankAccount();
            account.BankName = string.Empty;

            // Assert:
            Assert.Throws<InvalidBankNameException>(() => _repository.Update(account));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteAndReturnBankAccount_WhenBankAccountExists()
        {
            // Arrange:
            BankAccount account = GenerateCompleteBankAccount();

            await _dbContext.BankAccounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            _dbContext.ChangeTracker.Clear();

            // Act:
            BankAccount result = await _repository.DeleteAsync(account.Id, _cancellationToken);
            await _dbContext.SaveChangesAsync();

            BankAccount? persisted = await _dbContext.BankAccounts.FindAsync(account.Id);

            // Assert:
            Assert.Equal(account.Id, result.Id);
            Assert.Null(persisted);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowArgumentException_WhenBankAccountIdIsEmpty()
        {
            // Assert, Act: & Arrange:
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.DeleteAsync(Guid.Empty, _cancellationToken));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowBankAccountNotFoundException_WhenBankAccountDoesNotExist()
        {
            // Assert, Act: & Arrange:
            await Assert.ThrowsAsync<BankAccountNotFoundException>(() => _repository.DeleteAsync(Guid.NewGuid(), _cancellationToken));
        }
    }
}