namespace Users.Domain.Exceptions.UserExceptions
{
    public sealed class InvalidUserPhoneException : DomainException
    {
        public string Phone { get; }

        // Constructors:
        // Constructor with phone:
        public InvalidUserPhoneException(string phone) : base($"The phone '{phone}' is invalid!") => Phone = phone;

        // Constructor with phone and custom message:
        public InvalidUserPhoneException(string phone, string message) : base(message) => Phone = phone;

        // Constructor with phone, custom message and inner exception:
        public InvalidUserPhoneException(string phone, string message, Exception innerException) : base(message, innerException) => Phone = phone;
    }
}