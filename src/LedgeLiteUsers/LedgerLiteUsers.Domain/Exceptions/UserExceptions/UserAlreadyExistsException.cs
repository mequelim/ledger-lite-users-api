namespace LedgeLiteUsers.Domain.Errors.UserExceptions
{
    public sealed class UserAlreadyExistsException : DomainException
    {
        public Guid UserId { get; }

        // Constructors:
        // Constructor with userId:
        public UserAlreadyExistsException(Guid userId) : base($"User with id '{userId}' is already active!") => UserId = userId;

        // Constructor with userId and custom message:
        public UserAlreadyExistsException(Guid userId, string message) : base(message) => UserId = userId;

        // Constructor with userId, custom message and inner exception:
        public UserAlreadyExistsException(Guid userId, string message, Exception innerException) : base(message, innerException) => UserId = userId;
    }
}