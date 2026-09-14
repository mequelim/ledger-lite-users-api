using Microsoft.EntityFrameworkCore;
using Users.Persistence.Tests.Mocks.Auditable;

namespace Users.Persistence.Tests.Fixtures
{
    public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<TestAuditableEntity> AuditableEntities { get; set; } = null!;
        public DbSet<TestSoftDeletableEntity> SoftDeletableEntities { get; set; } = null!;
    }
}