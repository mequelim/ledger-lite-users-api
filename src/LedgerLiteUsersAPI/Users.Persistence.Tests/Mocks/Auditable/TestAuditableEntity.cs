using Users.Domain.Interfaces.Auditable;

namespace Users.Persistence.Tests.Mocks.Auditable
{
    public class TestAuditableEntity : IAuditableEntity
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; set; } = "Initial";
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
    }
}