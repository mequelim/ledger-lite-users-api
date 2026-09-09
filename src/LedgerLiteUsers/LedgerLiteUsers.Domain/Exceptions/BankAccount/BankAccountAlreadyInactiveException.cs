namespace LedgeLiteUsers.Domain.Errors.BankAccount
{
    public class BankAccountAlreadyInactiveException : DomainException
    {
        public Guid BankAccountId { get; }

        // Constructor:
        // Constructor with bankAccountId:
        public BankAccountAlreadyInactiveException(Guid bankAccountId) : base($"Bank account with id '{bankAccountId}' is already inactive!")
        {
            BankAccountId = bankAccountId;
        }

        // Constructor with bankAccount and custom message:
        public BankAccountAlreadyInactiveException(Guid bankAccountId, string message) : base(message) => BankAccountId = bankAccountId;

        // Constructor with bankAccountId, custom message and inner exception:
        public BankAccountAlreadyInactiveException(Guid bankAccountId, string message, Exception innerException) : base(message, innerException)
        {
            BankAccountId = bankAccountId;
        }
    }
}