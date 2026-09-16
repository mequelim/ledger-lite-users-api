using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Domain.Exceptions.UserExceptions;
using Users.Persistence.Database;
using Users.Persistence.Repositories;
using Users.Persistence.Tests.Fixtures;
using Users.Persistence.Tests.Mocks.Domain;

namespace Users.Persistence.Tests.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly AppDbContext _dbContext;
        private readonly UserRepository _repository;

        // Constructor:
        public UserRepositoryTests()
        {
            _dbContext = PersistenceTestContext.CreateInMemoryDbContext();
            _repository = new UserRepository(_dbContext);
        }

        // Methods:
        private User GenerateCompleteUser() => new UserBuilder().Build();

        private async Task<User> PersistUserAsync()
        {
            User user = GenerateCompleteUser();

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        // Tests:
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers_WhenMultipleUsersExist()
        {
            // Arrange:
            User user01 = GenerateCompleteUser();
            User user02 = GenerateCompleteUser();

            user02.Email = "user02@email.com";
            user02.Phone = "11999999999";

            await _dbContext.Users.AddRangeAsync(user01, user02);
            await _dbContext.SaveChangesAsync();

            // Act:
            IEnumerable<User> result = await _repository.GetAllAsync();

            // Assert:
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetActiveUsersAsync_ShouldReturnOnlyActiveUsers_WhenActiveUsersExist()
        {
            // Arrange:
            User activeUser = GenerateCompleteUser();

            User inactiveUser = GenerateCompleteUser();
            inactiveUser.Email = "inactive@email.com";
            inactiveUser.Phone = "11888888888";
            inactiveUser.IsActive = false;

            await _dbContext.Users.AddRangeAsync(activeUser, inactiveUser);
            await _dbContext.SaveChangesAsync();

            // Act:
            IEnumerable<User> result = await _repository.GetActiveUsersAsync();

            // Assert:
            IEnumerable<User> users = result as User[] ?? [.. result];

            Assert.Single(users);
            Assert.All(users, user => Assert.True(user.IsActive));
        }

        [Fact]
        public async Task GetInactiveUsersAsync_ShouldReturnOnlyInactiveUsers_WhenInactiveUsersExist()
        {
            // Arrange:
            User activeUser = new UserBuilder().Build();

            User inactiveUser = new UserBuilder().Build();
            inactiveUser.IsActive = false;
            inactiveUser.Email = "inactive@email.com";

            await _dbContext.Users.AddRangeAsync(activeUser, inactiveUser);
            await _dbContext.SaveChangesAsync();

            // Act:
            IEnumerable<User> result = await _repository.GetInactiveUsersAsync();
            IEnumerable<User> users = result as User[] ?? [.. result];

            // Assert:
            Assert.Single(users);
            Assert.All(users, user => Assert.False(user.IsActive));
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnAUser_WhenUserExists()
        {
            // Arrange:
            User user = await PersistUserAsync();

            // Act:
            User result = await _repository.GetUserByIdAsync(user.Id);

            // Assert:
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Surname, result.Surname);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Phone, result.Phone);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
        {
            // Assert, Act & Arrange:
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetUserByIdAsync(Guid.Empty));
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange & Act:
            User? result = await _repository.GetUserByIdAsync(Guid.NewGuid());

            // Assert:
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByNameOrSurnameAsync_ShouldReturnUsers_WhenNameMatches()
        {
            // Arrange:
            User user = await PersistUserAsync();

            // Act:
            IEnumerable<User> result = await _repository.GetUserByNameOrSurnameAsync("Pedro");

            // Assert:
            Assert.Contains(result, u => u.Id == user.Id);
        }

        [Fact]
        public async Task GetUserByNameOrSurnameAsync_ShouldReturnUsers_WhenSurnameMatches()
        {
            // Arrange:
            User user = await PersistUserAsync();

            // Act:
            IEnumerable<User> result = await _repository.GetUserByNameOrSurnameAsync("Mequelim");

            // Assert:
            Assert.Contains(result, u => u.Id == user.Id);
        }

        [Fact]
        public async Task GetUserByNameOrSurnameAsync_ShouldReturnEmpty_WhenNoUserMatches()
        {
            // Arrange:
            await PersistUserAsync();

            // Act:
            IEnumerable<User> result = await _repository.GetUserByNameOrSurnameAsync("Fernanda");

            // Assert:
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetUserByNameOrSurnameAsync_ShouldThrowArgumentException_WhenNameIsNullOrWhitespace(string? name)
        {
            // Assert, Act & Arrange:
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetUserByNameOrSurnameAsync(name!));
        }

        [Fact]
        public async Task GetUserByFullNameAsync_ShouldReturnUsers_WhenFullNameMatches()
        {
            // Arrange:
            User user = await PersistUserAsync();

            // Act:
            IEnumerable<User> result = await _repository.GetUserByFullNameAsync("Pedro Mequelim");

            // Assert:
            Assert.Contains(result, u => u.Id == user.Id);
        }

        [Fact]
        public async Task GetUserByFullNameAsync_ShouldReturnEmpty_WhenNoUserMatches()
        {
            // Arrange:
            await PersistUserAsync();

            // Act:
            IEnumerable<User> result = await _repository.GetUserByFullNameAsync("Fernanda Silva");

            // Assert:
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetUserByFullNameAsync_ShouldThrowArgumentException_WhenFullNameIsNullOrWhitespace(string? fullName)
        {
            // Assert, Act & Arrange:
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetUserByFullNameAsync(fullName!));
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldReturnAUser_WhenEmailExists()
        {
            // Arrange:
            User user = await PersistUserAsync();

            // Act:
            User result = await _repository.GetUserByEmailAsync(user.Email);

            // Assert:
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email, result.Email);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetUserByEmailAsync_ShouldThrowArgumentException_WhenEmailIsNullOrWhitespace(string? email)
        {
            // Assert, Act & Arrange:
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetUserByEmailAsync(email!));
        }

        [Fact]
        public async Task GetUserByEmailAsync_ShouldThrowInvalidUserEmailException_WhenEmailDoesNotExist()
        {
            // Arrange & Act:
            User? result = await _repository.GetUserByEmailAsync("notfound@email.com");

            // Assert:
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByPhoneAsync_ShouldReturnAUser_WhenPhoneExists()
        {
            // Arrange:
            User user = await PersistUserAsync();

            // Act:
            User result = await _repository.GetUserByPhoneAsync(user.Phone);

            // Assert:
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Phone, result.Phone);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetUserByPhoneAsync_ShouldThrowArgumentException_WhenPhoneIsNullOrWhitespace(string? phone)
        {
            // Assert, Act & Arrange:
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetUserByPhoneAsync(phone!));
        }

        [Fact]
        public async Task GetUserByPhoneAsync_ShouldThrowInvalidUserPhoneException_WhenPhoneDoesNotExist()
        {
            // Assert, Act & Arrange:
            await Assert.ThrowsAsync<InvalidUserPhoneException>(() => _repository.GetUserByPhoneAsync("11000000000"));
        }

        [Fact]
        public async Task Create_ShouldPersistAndReturnUser_WhenDataIsValid()
        {
            User user = GenerateCompleteUser();
            User result = _repository.Create(user);
            await _dbContext.SaveChangesAsync();

            Assert.Equal(user.Id, result.Id);
            _dbContext.ChangeTracker.Clear();
            Assert.NotNull(await _dbContext.Users.FindAsync(user.Id));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndReturnUser_WhenDataIsValid()
        {
            // Arrange:
            User user = GenerateCompleteUser();

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            user.Name = "João";

            // Act:
            User result = _repository.Update(user);
            User? persisted = await _dbContext.Users.FindAsync(user.Id);

            // Assert:
            Assert.Equal("João", result.Name);
            Assert.Equal("João", persisted!.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteAndReturnUser_WhenUserExists()
        {
            User user = GenerateCompleteUser();
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            _dbContext.ChangeTracker.Clear();

            User result = await _repository.DeleteAsync(user.Id);
            await _dbContext.SaveChangesAsync();

            Assert.Equal(user.Id, result.Id);
            Assert.Null(await _dbContext.Users.FindAsync(user.Id));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
        {
            // Assert, Act & Arrange:
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.DeleteAsync(Guid.Empty));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
        {
            // Assert, Act & Arrange:
            await Assert.ThrowsAsync<UserNotFoundException>(() => _repository.DeleteAsync(Guid.NewGuid()));
        }
    }
}