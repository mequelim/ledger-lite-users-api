namespace Users.Domain.Exceptions.BankAccount
{
    public sealed class BankAccountNotFoundException : DomainException
    {
        public Guid BankAccountId { get; }

        // Constructor:
        // Constructor with bankAccountId:
        public BankAccountNotFoundException(Guid bankAccountId) : base($"Bank account with id '{bankAccountId}' was not found!")
        {
            BankAccountId = bankAccountId;
        }

        // Constructor with bankAccount and custom message:
        public BankAccountNotFoundException(Guid bankAccountId, string message) : base(message) => BankAccountId = bankAccountId;

        // Constructor with bankAccountId, custom message and inner exception:
        public BankAccountNotFoundException(Guid bankAccountId, string message, Exception innerException) : base(message, innerException)
        {
            BankAccountId = bankAccountId;
        }
    }
}