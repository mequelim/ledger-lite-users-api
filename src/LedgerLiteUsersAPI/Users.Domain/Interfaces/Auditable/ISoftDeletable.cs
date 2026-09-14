namespace Users.Domain.Interfaces.Auditable
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        Guid? DeletedBy { get; set; }
        DateTime? DeletedOnUtc { get; set; }
    }
}