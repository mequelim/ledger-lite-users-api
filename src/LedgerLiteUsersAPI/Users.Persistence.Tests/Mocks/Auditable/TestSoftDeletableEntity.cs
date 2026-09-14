namespace Users.Persistence.Tests.Mocks.Auditable
{
    public class TestSoftDeletableEntity
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public bool IsDeleted { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedOnUtc { get; set; }
    }
}