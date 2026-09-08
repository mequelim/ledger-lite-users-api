namespace LedgeLiteUsers.Domain.Errors.UserExceptions
{
    public sealed class UserAlreadyActiveException : DomainException
    {
        public Guid UserId { get; }

        // Constructors:
        // Constructor with userId:
        public UserAlreadyActiveException(Guid userId) : base($"User with id '{userId}' was not found!") => UserId = userId;

        // Constructor with userId and custom message:
        public UserAlreadyActiveException(Guid userId, string message) : base(message) => UserId = userId;

        // Constructor with userId, custom message and inner exception:
        public UserAlreadyActiveException(Guid userId, string message, Exception innerException) : base(message, innerException) => UserId = userId;
    }
}