namespace LedgeLiteUsers.Domain.Errors.UserExceptions
{
    public sealed class UserAlreadyInactiveException : DomainException
    {
        public Guid UserId { get; }

        // Constructors:
        // Constructor with userId:
        public UserAlreadyInactiveException(Guid userId) : base($"User with id '{userId}' is already inactive!") => UserId = userId;

        // Constructor with userId and custom message:
        public UserAlreadyInactiveException(Guid userId, string message) : base(message) => UserId = userId;

        // Constructor with userId, custom message and inner exception:
        public UserAlreadyInactiveException(Guid userId, string message, Exception innerException) : base(message, innerException) => UserId = userId;
    }
}