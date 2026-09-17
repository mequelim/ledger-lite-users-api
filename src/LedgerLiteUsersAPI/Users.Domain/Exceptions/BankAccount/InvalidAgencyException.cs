namespace Users.Domain.Exceptions.BankAccount
{
    public sealed class InvalidAgencyException : DomainException
    {
        public string Agency { get; }

        // Constructors:
        // Constructor with agency:
        public InvalidAgencyException(string agency) : base($"The bank account's agency '{agency}' is invalid!")
        {
            Agency = agency;
        }

        // Constructor with agency and custom message:
        public InvalidAgencyException(string agency, string message) : base(message) => Agency = agency;

        // Constructor with agency, custom message and inner exception:
        public InvalidAgencyException(string agency, string message, Exception innerException) : base(message, innerException)
        {
            Agency = agency;
        }
    }
}