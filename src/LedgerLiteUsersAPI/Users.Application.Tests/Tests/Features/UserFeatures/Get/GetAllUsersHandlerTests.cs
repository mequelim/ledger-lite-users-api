using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.UserFeatures.Get.GetAll;
using Users.Domain.Entities;
using Users.Domain.Entities.Enums;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Get
{
    public class GetAllUsersHandlerTests : IDisposable
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _databaseContext;
        private readonly GetAllUsersHandler _userHandler;

        // Constructor:
        public GetAllUsersHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);
            _userHandler = new GetAllUsersHandler(_userRepository);
        }

        // Methods:
        private static User CreateValidUser(Guid userId, string name, string surname, bool isActive)
        {
            User user = new(
                name: name,
                surname: surname,
                birthdate: new DateOnly(1998, 5, 10),
                email: $"{name.ToLower()}@email.com",
                phone: "11999999999",
                isActive: isActive
            );

            typeof(User)
                .GetProperty(nameof(User.Id))!
                .SetValue(user, userId);

            BankAccount bankAccount = new(
                bankName: "Nubank",
                holder: $"{name} {surname}",
                accountNumber: "12345678-9",
                agency: "0001",
                bankAccountType: BankAccountType.Checking,
                userId: userId
            );

            user.AddBankAccount(bankAccount);

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
        public async Task Handle_ShouldReturnSuccess_WhenThereAreNoUsers()
        {
            // Arrange:
            GetAllUsersQuery query = new();

            _userRepository
                .GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(Enumerable.Empty<User>());

            // Act:
            Result<GetAllUsersResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();
            result.Value.Should().NotBeNull();
            result.Value.Users.Should().BeEmpty();

            await _userRepository
                .Received(1)
                .GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUsersExist()
        {
            // Arrange:
            GetAllUsersQuery query = new();
            List<User> users =
            [
                CreateValidUser(Guid.NewGuid(), "Pedro", "Henrique", true),
                CreateValidUser(Guid.NewGuid(), "Maria", "Silva", false)
            ];

            _userRepository
                .GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(users);

            // Act:
            Result<GetAllUsersResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Users.Should().HaveCount(2);
            result.Value.Users.Should().BeEquivalentTo(users);

            result.Value.Users.First().BankAccounts.Should().HaveCount(1);
            result.Value.Users.First().BankAccounts.First().BankName.Should().Be("Nubank");

            await _userRepository
                .Received(1)
                .GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnlyOnce_WhenQueryIsExecuted()
        {
            // Arrange:
            GetAllUsersQuery query = new();
            List<User> users =
            [
                CreateValidUser(Guid.NewGuid(), "Pedro", "Henrique", true)
            ];

            _userRepository
                .GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(users);

            // Act:
            await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            await _userRepository
                .Received(1)
                .GetAllAsync(Arg.Any<CancellationToken>());
        }
    }
}