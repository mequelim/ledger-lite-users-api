namespace Users.Domain.Exceptions.UserExceptions
{
    public sealed class UserInactiveException : DomainException
    {
        public Guid UserId { get; }

        // Constructors:
        // Constructor with userId:
        public UserInactiveException(Guid userId) : base($"User with id '{userId}' is inactive!") => UserId = userId;

        // Constructor with userId and custom message:
        public UserInactiveException(Guid userId, string message) : base(message) => UserId = userId;

        // Constructor with userId, custom message and inner exception:
        public UserInactiveException(Guid userId, string message, Exception innerException) : base(message, innerException) => UserId = userId;
    }
}