namespace Users.Domain.Exceptions.UserExceptions
{
    public sealed class UserNotFoundException : DomainException
    {
        public string PropertyName { get; }
        public object PropertyValue { get; }

        // Constructors:
        // Generic constructor:
        public UserNotFoundException(string propertyName, object propertyValue) : base($"User with {propertyName} '{propertyValue}' was not found!")
        {
            (PropertyName, PropertyValue) = (propertyName, propertyValue);
        }

        // Constructor with a custom message:
        public UserNotFoundException(string propertyName, object propertyValue, string message) : base(message)
        {
            (PropertyName, PropertyValue) = (propertyName, propertyValue);
        }

        // Constructor with message and inner exception:
        public UserNotFoundException(string propertyName, object propertyValue, string message, Exception innerException) : base(message, innerException)
        {
            (PropertyName, PropertyValue) = (propertyName, propertyValue);
        }
    }
}