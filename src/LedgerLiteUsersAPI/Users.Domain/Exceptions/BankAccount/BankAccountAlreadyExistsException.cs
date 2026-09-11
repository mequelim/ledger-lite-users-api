namespace Users.Domain.Exceptions.BankAccount
{
    public sealed class BankAccountAlreadyExists : DomainException
    {
        public Guid BankAccountId { get; }

        // Constructor:
        // Constructor with bankAccountId:
        public BankAccountAlreadyExists(Guid bankAccountId) : base($"Bank account with id '{bankAccountId}' already exists!")
        {
            BankAccountId = bankAccountId;
        }

        // Constructor with bankAccount and custom message:
        public BankAccountAlreadyExists(Guid bankAccountId, string message) : base(message) => BankAccountId = bankAccountId;

        // Constructor with bankAccountId, custom message and inner exception:
        public BankAccountAlreadyExists(Guid bankAccountId, string message, Exception innerException) : base(message, innerException)
        {
            BankAccountId = bankAccountId;
        }
    }
}