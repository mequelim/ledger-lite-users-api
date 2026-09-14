using Microsoft.EntityFrameworkCore;
using Users.Persistence.Database;

namespace Users.Persistence.Tests.Fixtures
{
    public static class PersistenceTestContext
    {
        public static AppDbContext CreateInMemoryDbContext()
        {
            DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}