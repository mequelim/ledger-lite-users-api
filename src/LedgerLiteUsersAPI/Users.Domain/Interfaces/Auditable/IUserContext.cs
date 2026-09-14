namespace Users.Domain.Interfaces.Auditable
{
    public interface IUserContext
    {
        Guid? UserId { get; }
    }
}