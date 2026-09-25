using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.UserFeatures.Get.GetByUserName;
using Users.Application.Tests.Mocks;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Get
{
    public class GetUserByUserNameHandlerTests : IDisposable
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _databaseContext;
        private readonly GetUserByUserNameHandler _userHandler;

        // Constructor:
        public GetUserByUserNameHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);
            _userHandler = new GetUserByUserNameHandler(_userRepository);
        }

        // Methods:
        private static GetUserByUserNameQuery CreateValidQuery(string userName) => new(userName);

        private static User CreateValidUser(Guid userId, string name, string surname)
        {
            User user = new UserBuilder().Build();

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
        public async Task Handle_ShouldReturnSuccess_WhenUserNameHasNoMatches()
        {
            // Arrange:
            const string userName = "Pedro";

            GetUserByUserNameQuery query = CreateValidQuery(userName);

            _userRepository
                .GetUserByNameAsync(query.UserName, Arg.Any<CancellationToken>())
                .Returns(Enumerable.Empty<User>());

            // Act:
            Result<GetUserByUserNameResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Users.Should().NotBeNull();
            result.Value.Users.Should().BeEmpty();

            await _userRepository
                .Received(1)
                .GetUserByNameAsync(userName, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserNameHasMatches()
        {
            // Arrange:
            const string userName = "Pedro";

            GetUserByUserNameQuery query = CreateValidQuery(userName);
            List<User> users =
            [
                CreateValidUser(Guid.NewGuid(), "Pedro", "Henrique"),
                CreateValidUser(Guid.NewGuid(), "Pedro", "Silva")
            ];

            _userRepository
                .GetUserByNameAsync(query.UserName, Arg.Any<CancellationToken>())
                .Returns(users);

            // Act:
            Result<GetUserByUserNameResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Users.Should().HaveCount(2);
            result.Value.Users.Should().BeEquivalentTo(users);

            await _userRepository
                .Received(1)
                .GetUserByNameAsync(userName, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnlyOnce_WhenUserNameIsValid()
        {
            // Arrange:
            const string userName = "Pedro";

            GetUserByUserNameQuery query = CreateValidQuery(userName);
            List<User> users =
            [
                CreateValidUser(Guid.NewGuid(), "Pedro", "Henrique")
            ];

            _userRepository
                .GetUserByNameAsync(query.UserName, Arg.Any<CancellationToken>())
                .Returns(users);

            // Act:
            await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            await _userRepository
                .Received(1)
                .GetUserByNameAsync(userName, Arg.Any<CancellationToken>());
        }
    }
}