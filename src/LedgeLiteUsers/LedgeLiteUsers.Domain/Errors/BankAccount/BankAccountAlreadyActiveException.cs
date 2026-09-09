namespace LedgeLiteUsers.Domain.Errors.BankAccount
{
    public sealed class BankAccountAlreadyActiveException : DomainException
    {
        public Guid BankAccountId { get; }

        // Constructor:
        // Constructor with bankAccountId:
        public BankAccountAlreadyActiveException(Guid bankAccountId) : base($"Bank account with id '{bankAccountId}' was not found!")
        {
            BankAccountId = bankAccountId;
        }

        // Constructor with bankAccount and custom message:
        public BankAccountAlreadyActiveException(Guid bankAccountId, string message) : base(message) => BankAccountId = bankAccountId;

        // Constructor with bankAccountId, custom message and inner exception:
        public BankAccountAlreadyActiveException(Guid bankAccountId, string message, Exception innerException) : base(message, innerException)
        {
            BankAccountId = bankAccountId;
        }
    }
}