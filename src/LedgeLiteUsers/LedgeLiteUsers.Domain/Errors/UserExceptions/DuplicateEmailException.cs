namespace LedgeLiteUsers.Domain.Errors.UserExceptions
{
    public sealed class DuplicateEmailException : DomainException
    {
        public string Email { get; }

        // Constructors:
        // Constructor with email:
        public DuplicateEmailException(string email) : base($"The e-mail '{email}' already exists!") => Email = email;

        // Constructor with email and custom message:
        public DuplicateEmailException(string email, string message) : base(message) => Email = email;

        // Constructor with email, custom message and inner exception:
        public DuplicateEmailException(string email, string message, Exception innerException) : base(message, innerException) => Email = email;
    }
}