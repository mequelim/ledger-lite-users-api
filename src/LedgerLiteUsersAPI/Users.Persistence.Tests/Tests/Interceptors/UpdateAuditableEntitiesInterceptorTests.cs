using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Domain.Interfaces.Auditable;
using Users.Persistence.Interceptors;
using Users.Persistence.Tests.Fixtures;
using Users.Persistence.Tests.Mocks.Auditable;

namespace Users.Persistence.Tests.Tests.Interceptors
{
    public class UpdateAuditableEntitiesInterceptorTests
    {
        private readonly IUserContext _userContextMock;
        private readonly TestDbContext _context;

        public UpdateAuditableEntitiesInterceptorTests()
        {
            _userContextMock = Substitute.For<IUserContext>();

            UpdateAuditableEntitiesInterceptor interceptor = new(_userContextMock);
            DbContextOptions<TestDbContext> options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .AddInterceptors(interceptor)
                .Options;

            _context = new TestDbContext(options);
        }

        // Tests:
        [Fact]
        public async Task SaveChangesAsync_ShouldSetCreationAuditFields_WhenEntityIsAdded()
        {
            // Arrange:
            Guid currentUserId = Guid.NewGuid();
            _userContextMock.UserId.Returns(currentUserId);

            TestAuditableEntity entity = new TestAuditableEntity();

            // Act:
            await _context.AuditableEntities.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Assert:
            Assert.Equal(currentUserId, entity.CreatedBy);
            Assert.True((DateTime.UtcNow - entity.CreatedOnUtc).TotalSeconds < 5);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldSetModificationAuditField_WhenEntityIsModified()
        {
            // Arrange:
            Guid currentUserId = Guid.NewGuid();
            _userContextMock.UserId.Returns(currentUserId);

            TestAuditableEntity entity = new();
            await _context.AuditableEntities.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Act:
            entity.Name = "Updated Name";
            await _context.SaveChangesAsync();

            // Assert:
            Assert.Equal(currentUserId, entity.ModifiedBy);
            Assert.NotNull(entity.ModifiedOnUtc);
            Assert.True((DateTime.UtcNow - entity.ModifiedOnUtc!.Value).TotalSeconds < 5);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldConvertToModifiedAndSetDeletedAuditFields_WhenSoftDeletableEntityIsDeleted()
        {
            // Arrange:
            Guid currentUserId = Guid.NewGuid();
            _userContextMock.UserId.Returns(currentUserId);

            TestSoftDeletableEntity entity = new();
            await _context.SoftDeletableEntities.AddAsync(entity);
            await _context.SaveChangesAsync();

            // Act:
            _context.SoftDeletableEntities.Remove(entity);
            await _context.SaveChangesAsync();

            // Assert:
            Assert.True(entity.IsDeleted);
            Assert.Equal(currentUserId, entity.DeletedBy);
            Assert.NotNull(entity.DeletedOnUtc);
            Assert.True((DateTime.UtcNow - entity.DeletedOnUtc!.Value).TotalSeconds < 5);

            TestSoftDeletableEntity? dbEntity = await _context.SoftDeletableEntities.FindAsync(entity.Id);
            Assert.NotNull(dbEntity);
        }
    }
}