using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.UserFeatures.Get.GetById;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Get
{
    public class GetUserByIdHandlerTests : IDisposable
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _databaseContext;
        private readonly GetUserByIdHandler _userHandler;

        // Constructor:
        public GetUserByIdHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);
            _userHandler = new GetUserByIdHandler(_userRepository);
        }

        // Methods:
        private static GetUserByIdQuery CreateValidQuery(Guid userId) => new(userId);

        private static User CreateValidUser(Guid userId)
        {
            User user = new(
                name: "Pedro",
                surname: "Henrique",
                birthdate: new DateOnly(1998, 5, 10),
                email: "pedro@email.com",
                phone: "11999999999",
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
        public async Task Handle_ShouldReturnFailure_WhenUserIdIsEmpty()
        {
            // Arrange:
            GetUserByIdQuery query = new(Guid.Empty);

            // Act:
            Result<GetUserByIdResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("The user id cannot be null!");

            await _userRepository
                .DidNotReceive()
                .GetUserByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();
            GetUserByIdQuery query = CreateValidQuery(userId);

            _userRepository
                .GetUserByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act:
            Result<GetUserByIdResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found!");

            await _userRepository
                .Received(1)
                .GetUserByIdAsync(userId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserExists()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();
            GetUserByIdQuery query = CreateValidQuery(userId);
            User user = CreateValidUser(userId);

            _userRepository
                .GetUserByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act:
            Result<GetUserByIdResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(userId);

            await _userRepository
                .Received(1)
                .GetUserByIdAsync(userId, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnlyOnce_WhenUserExists()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();
            GetUserByIdQuery query = CreateValidQuery(userId);
            User user = CreateValidUser(userId);

            _userRepository
                .GetUserByIdAsync(query.Id, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act:
            await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            await _userRepository
                .Received(1)
                .GetUserByIdAsync(userId, Arg.Any<CancellationToken>());
        }
    }
}