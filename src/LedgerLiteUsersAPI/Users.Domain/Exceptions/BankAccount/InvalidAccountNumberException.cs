namespace Users.Domain.Exceptions.BankAccount
{
    public sealed class InvalidAccountNumberException : DomainException
    {
        public string AccountNumber { get; }

        // Constructors:
        // Constructor with agency:
        public InvalidAccountNumberException(string accountNumber) : base($"The account's number '{accountNumber}' is invalid!")
        {
            AccountNumber = accountNumber;
        }

        // Constructor with agency and custom message:
        public InvalidAccountNumberException(string accountNumber, string message) : base(message) => AccountNumber = accountNumber;

        // Constructor with agency, custom message and inner exception:
        public InvalidAccountNumberException(string accountNumber, string message, Exception innerException) : base(message, innerException)
        {
            AccountNumber = accountNumber;
        }
    }
}