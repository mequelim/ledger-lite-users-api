using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.UserFeatures.Get.GetAllActiveUsers;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Get
{
    public class GetAllActiveUsersHandlerTests : IDisposable
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _databaseContext;
        private readonly GetAllActiveUsersHandler _userHandler;

        // Constructor:
        public GetAllActiveUsersHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);
            _userHandler = new GetAllActiveUsersHandler(_userRepository);
        }

        // Methods:
        private static User CreateValidUser(Guid userId, string name, bool isActive)
        {
            User user = new(
                name: name,
                surname: "Henrique",
                birthdate: new DateOnly(1998, 5, 10),
                email: $"{name.ToLower()}@email.com",
                phone: "11999999999",
                isActive: isActive
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
        public async Task Handle_ShouldReturnSuccess_WhenThereAreNoActiveUsers()
        {
            // Arrange:
            GetAllActiveUsersQuery query = new();

            _userRepository
                .GetAllActiveUsersAsync(Arg.Any<CancellationToken>())
                .Returns(Enumerable.Empty<User>());

            // Act:
            Result<GetAllActiveUsersResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Users.Should().BeEmpty();

            await _userRepository
                .Received(1)
                .GetAllActiveUsersAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenActiveUsersExist()
        {
            // Arrange:
            GetAllActiveUsersQuery query = new();
            List<User> users =
            [
                CreateValidUser(Guid.NewGuid(), "Pedro", true),
                CreateValidUser(Guid.NewGuid(), "Maria", true)
            ];

            _userRepository
                .GetAllActiveUsersAsync(Arg.Any<CancellationToken>())
                .Returns(users);

            // Act:
            Result<GetAllActiveUsersResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Users.Should().HaveCount(2);
            result.Value.Users.Should().BeEquivalentTo(users);

            result.Value.Users.Should().OnlyContain(user => user.IsActive == true);

            await _userRepository
                .Received(1)
                .GetAllActiveUsersAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnlyOnce_WhenQueryIsExecuted()
        {
            // Arrange:
            GetAllActiveUsersQuery query = new();
            List<User> users =
            [
                CreateValidUser(Guid.NewGuid(), "Pedro", true)
            ];

            _userRepository
                .GetAllActiveUsersAsync(Arg.Any<CancellationToken>())
                .Returns(users);

            // Act:
            await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            await _userRepository
                .Received(1)
                .GetAllActiveUsersAsync(Arg.Any<CancellationToken>());
        }
    }
}