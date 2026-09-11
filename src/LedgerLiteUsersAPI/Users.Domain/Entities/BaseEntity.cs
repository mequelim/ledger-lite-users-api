using Users.Domain.Interfaces.Auditable;

namespace Users.Domain.Entities
{
    public class BaseEntity(Guid id) : IAuditableEntity
    {
        public Guid Id { get; protected set; } = id;
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }

        protected BaseEntity() : this(Guid.NewGuid()) {}
    }
}