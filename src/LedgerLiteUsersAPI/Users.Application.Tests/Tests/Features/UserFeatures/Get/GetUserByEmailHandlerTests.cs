using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.UserFeatures.Get.GetByEmail;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Get
{
    public class GetUserByEmailHandlerTests : IDisposable
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _databaseContext;
        private readonly GetUserByEmailHandler _userHandler;

        // Constructor:
        public GetUserByEmailHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);
            _userHandler = new GetUserByEmailHandler(_userRepository);
        }

        // Methods:
        private static GetUserByEmailQuery CreateValidQuery(string email) => new(email);

        private static User CreateValidUser(Guid userId, string email)
        {
            User user = new(
                name: "Pedro",
                surname: "Henrique",
                birthdate: new DateOnly(1998, 5, 10),
                email: email,
                phone: "11987654321",
                isActive: true
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
        public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Arrange:
            const string email = "pedro@email.com";

            GetUserByEmailQuery query = CreateValidQuery(email);

            _userRepository
                .GetUserByEmailAsync(query.Email, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act:
            Result<GetUserByEmailResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found!");

            await _userRepository
                .Received(1)
                .GetUserByEmailAsync(email, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserExists()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();
            const string email = "pedro@email.com";

            GetUserByEmailQuery query = CreateValidQuery(email);

            User user = CreateValidUser(userId, email);

            _userRepository
                .GetUserByEmailAsync(query.Email, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act:
            Result<GetUserByEmailResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.User.Should().BeEquivalentTo(user);

            await _userRepository
                .Received(1)
                .GetUserByEmailAsync(email, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnlyOnce_WhenEmailIsValid()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();
            const string email = "pedro@email.com";

            GetUserByEmailQuery query = CreateValidQuery(email);

            User user = CreateValidUser(userId, email);

            _userRepository
                .GetUserByEmailAsync(query.Email, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act:
            await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            await _userRepository
                .Received(1)
                .GetUserByEmailAsync(email, Arg.Any<CancellationToken>());
        }
    }
}