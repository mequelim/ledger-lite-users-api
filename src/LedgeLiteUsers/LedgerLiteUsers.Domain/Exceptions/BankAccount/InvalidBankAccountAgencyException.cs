namespace LedgeLiteUsers.Domain.Errors.BankAccount
{
    public sealed class InvalidBankAccountAgencyException : DomainException
    {
        public string Agency { get; }

        // Constructors:
        // Constructor with agency:
        public InvalidBankAccountAgencyException(string agency) : base($"The bank account's agency '{agency}' is invalid!")
        {
            Agency = agency;
        }

        // Constructor with agency and custom message:
        public InvalidBankAccountAgencyException(string agency, string message) : base(message) => Agency = agency;

        // Constructor with agency, custom message and inner exception:
        public InvalidBankAccountAgencyException(string agency, string message, Exception innerException) : base(message, innerException)
        {
            Agency = agency;
        }
    }
}