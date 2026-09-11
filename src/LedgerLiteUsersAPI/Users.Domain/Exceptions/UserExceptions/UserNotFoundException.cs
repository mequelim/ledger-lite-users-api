namespace Users.Domain.Exceptions.UserExceptions
{
    public sealed class UserNotFoundException : DomainException
    {
        public Guid UserId { get; }

        // Constructors:
        // Constructor with userId:
        public UserNotFoundException(Guid userId) : base($"User with id '{userId}' was not found!") => UserId = userId;

        // Constructor with userId and custom message:
        public UserNotFoundException(Guid userId, string message) : base(message) => UserId = userId;

        // Constructor with userId, custom message and inner exception:
        public UserNotFoundException(Guid userId, string message, Exception innerException) : base(message, innerException) => UserId = userId;
    }
}