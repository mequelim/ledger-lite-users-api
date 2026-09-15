namespace Users.Domain.Exceptions.BankAccount
{
    public sealed class BankAccountNotFoundException : DomainException
    {
        public string PropertyName { get; }
        public object PropertyValue { get; }

        // Constructors:
        // Generic constructor:
        public BankAccountNotFoundException(string propertyName, object propertyValue): base($"Bank account with {propertyName} '{propertyValue}' was not found!")
        {
            (PropertyName, PropertyValue) = (propertyName, propertyValue);
        }

        // Constructor with a custom message:
        public BankAccountNotFoundException(string propertyName, object propertyValue, string message) : base(message)
        {
            (PropertyName, PropertyValue) = (propertyName, propertyValue);
        }

        // Constructor with message and inner exception:
        public BankAccountNotFoundException(string propertyName, object propertyValue, string message, Exception innerException)
            : base(message, innerException)
        {
            (PropertyName, PropertyValue) = (propertyName, propertyValue);
        }
    }
}