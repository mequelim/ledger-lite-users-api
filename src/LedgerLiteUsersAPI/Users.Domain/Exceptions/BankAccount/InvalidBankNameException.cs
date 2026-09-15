namespace Users.Domain.Exceptions.BankAccount
{
    public class InvalidBankNameException : DomainException
    {
        public string BankName { get; }

        // Constructors:
        // Constructor with name:
        public InvalidBankNameException(string name) : base($"The name '{name}' is invalid!") => BankName = name;

        // Constructor with name and custom message:
        public InvalidBankNameException(string name, string message) : base(message) => BankName = name;

        // Constructor with name, custom message and inner exception:
        public InvalidBankNameException(string name, string message, Exception innerException) : base(message, innerException) => BankName = name;
    }
}