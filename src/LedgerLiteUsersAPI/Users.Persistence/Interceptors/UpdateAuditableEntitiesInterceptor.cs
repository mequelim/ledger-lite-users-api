using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Users.Domain.Interfaces.Auditable;

namespace Users.Persistence.Interceptors
{
    public sealed class UpdateAuditableEntitiesInterceptor(IUserContext userContext) : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default
        )
        {
            DbContext? dbContext = eventData.Context;

            if(dbContext is null) return base.SavingChangesAsync(eventData, result, cancellationToken);

            // Creation & Updating:
            IEnumerable<EntityEntry<IAuditableEntity>> auditableEntities = dbContext.ChangeTracker.Entries<IAuditableEntity>();

            foreach(EntityEntry<IAuditableEntity> entityEntry in auditableEntities)
            {
                if(entityEntry.State == EntityState.Added)
                {
                    entityEntry.Entity.CreatedBy = userContext.UserId;
                    entityEntry.Entity.CreatedOnUtc = DateTime.UtcNow;
                }

                if(entityEntry.State == EntityState.Modified)
                {
                    entityEntry.Entity.ModifiedBy = userContext.UserId;
                    entityEntry.Entity.ModifiedOnUtc = DateTime.UtcNow;
                }
            }

            // Soft Deletable:
            IEnumerable<EntityEntry<ISoftDeletable>> softDeleteEntries = dbContext.ChangeTracker.Entries<ISoftDeletable>();

            foreach(EntityEntry<ISoftDeletable> entityEntry in softDeleteEntries)
            {
                if(entityEntry.State == EntityState.Deleted)
                {
                    entityEntry.State = EntityState.Modified;  // Cancels the physical deletion and converts it into a modification.

                    entityEntry.Entity.IsDeleted = true;
                    entityEntry.Entity.DeletedBy = userContext.UserId;
                    entityEntry.Entity.DeletedOnUtc = DateTime.UtcNow;
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}