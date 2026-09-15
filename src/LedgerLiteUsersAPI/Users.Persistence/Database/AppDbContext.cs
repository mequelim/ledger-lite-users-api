using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Auditable;
using Users.Persistence.Configurations;

namespace Users.Persistence.Database
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<User> Users { get; set; }

        // Methods:
        /// <summary>
        /// Generates a lambda expression to filter out entities marked as deleted.
        /// This is used for entities implementing the <see cref="ISoftDeletable"/> interface to exclude logically deleted records from queries.
        /// </summary>
        /// <param name="entityType">The type of the entity for which the query filter should be created.</param>
        /// <returns>A lambda expression that represents the query filter, typically in the form of <c>e => !e.IsDeleted</c>.</returns>
        private static LambdaExpression ConvertIsDeletedFilter(Type entityType)
        {
            // Generates the lambda expression: e => !e.IsDeleted.
            ParameterExpression parameter = Expression.Parameter(entityType, "e");
            MemberExpression property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            ConstantExpression falseConstant = Expression.Constant(false);
            BinaryExpression comparison = Expression.Equal(property, falseConstant);

            return Expression.Lambda(comparison, parameter);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            modelBuilder.ApplyGlobalConventions();

            foreach(IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                if(typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(ConvertIsDeletedFilter(entityType.ClrType));
                }
            }
        }
    }
}