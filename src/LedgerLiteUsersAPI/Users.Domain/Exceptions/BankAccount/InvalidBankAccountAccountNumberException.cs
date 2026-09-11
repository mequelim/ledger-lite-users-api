namespace Users.Domain.Exceptions.BankAccount
{
    public sealed class InvalidBankAccountAccountNumberException : DomainException
    {
        public string AccountNumber { get; }

        // Constructors:
        // Constructor with agency:
        public InvalidBankAccountAccountNumberException(string accountNumber) : base($"The account's number '{accountNumber}' is invalid!")
        {
            AccountNumber = accountNumber;
        }

        // Constructor with agency and custom message:
        public InvalidBankAccountAccountNumberException(string accountNumber, string message) : base(message) => AccountNumber = accountNumber;

        // Constructor with agency, custom message and inner exception:
        public InvalidBankAccountAccountNumberException(string accountNumber, string message, Exception innerException) : base(message, innerException)
        {
            AccountNumber = accountNumber;
        }
    }
}