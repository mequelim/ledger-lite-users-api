namespace Users.Domain.Interfaces.Auditable
{
    public interface IAuditableEntity
    {
        Guid? CreatedBy { get; set; }
        DateTime CreatedOnUtc { get; set; }
        Guid? ModifiedBy { get; set; }
        DateTime? ModifiedOnUtc { get; set; }
    }
}