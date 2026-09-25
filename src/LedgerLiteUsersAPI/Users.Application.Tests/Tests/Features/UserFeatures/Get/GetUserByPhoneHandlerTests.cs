using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Application.Common.Results;
using Users.Application.Features.UserFeatures.Get.GetByPhone;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Get
{
    public class GetUserByPhoneHandlerTests : IDisposable
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _databaseContext;
        private readonly GetUserByPhoneHandler _userHandler;

        // Constructor:
        public GetUserByPhoneHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();

            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = new AppDbContext(options);
            _userHandler = new GetUserByPhoneHandler(_userRepository);
        }

        // Methods:
        private static GetUserByPhoneQuery CreateValidQuery(string phone) => new(phone);

        private static User CreateValidUser(Guid userId, string phone)
        {
            User user = new(
                name: "Pedro",
                surname: "Henrique",
                birthdate: new DateOnly(1998, 5, 10),
                email: "pedro@email.com",
                phone: phone,
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
            const string phone = "11987654321";

            GetUserByPhoneQuery query = CreateValidQuery(phone);

            _userRepository
                .GetUserByPhoneAsync(query.Phone, Arg.Any<CancellationToken>())
                .Returns((User?)null);

            // Act:
            Result<GetUserByPhoneResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("User not found!");

            await _userRepository
                .Received(1)
                .GetUserByPhoneAsync(phone, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUserExists()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();
            const string phone = "11987654321";

            GetUserByPhoneQuery query = CreateValidQuery(phone);
            User user = CreateValidUser(userId, phone);

            _userRepository
                .GetUserByPhoneAsync(query.Phone, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act:
            Result<GetUserByPhoneResponse> result = await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            result.IsSuccess.Should().BeTrue();
            result.ErrorMessage.Should().BeNullOrEmpty();

            result.Value.Should().NotBeNull();
            result.Value.User.Should().BeEquivalentTo(user);

            await _userRepository
                .Received(1)
                .GetUserByPhoneAsync(phone, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryOnlyOnce_WhenPhoneIsValid()
        {
            // Arrange:
            Guid userId = Guid.NewGuid();
            const string phone = "11987654321";

            GetUserByPhoneQuery query = CreateValidQuery(phone);
            User user = CreateValidUser(userId, phone);

            _userRepository
                .GetUserByPhoneAsync(query.Phone, Arg.Any<CancellationToken>())
                .Returns(user);

            // Act:
            await _userHandler.Handle(query, CancellationToken.None);

            // Assert:
            await _userRepository
                .Received(1)
                .GetUserByPhoneAsync(phone, Arg.Any<CancellationToken>());
        }
    }
}