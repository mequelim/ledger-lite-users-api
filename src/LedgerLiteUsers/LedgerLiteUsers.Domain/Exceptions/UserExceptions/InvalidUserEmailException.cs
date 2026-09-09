namespace LedgeLiteUsers.Domain.Errors.UserExceptions
{
    public sealed class InvalidUserEmailException : DomainException
    {
        public string Email { get; }

        // Constructors:
        // Constructor with email:
        public InvalidUserEmailException(string email) : base($"The e-mail '{email}' is invalid!") => Email = email;

        // Constructor with email and custom message:
        public InvalidUserEmailException(string email, string message) : base(message) => Email = email;

        // Constructor with email, custom message and inner exception:
        public InvalidUserEmailException(string email, string message, Exception innerException) : base(message, innerException) => Email = email;
    }
}