namespace Users.Domain.Exceptions.UserExceptions
{
    public sealed class InvalidUserAgeException : DomainException
    {
        public DateOnly Birthdate { get; }

        // Constructors:
        // Constructor with birthdate:
        public InvalidUserAgeException(DateOnly birthdate) : base($"The birthdate '{birthdate}' is invalid... you must be at least 18 years old!")
        {
            Birthdate = birthdate;
        }

        // Constructor with birthdate and custom message:
        public InvalidUserAgeException(DateOnly birthdate, string message) : base(message) => Birthdate = birthdate;

        // Constructor with birthdate, custom message and inner exception:
        public InvalidUserAgeException(DateOnly birthdate, string message, Exception innerException) : base(message, innerException)
        {
            Birthdate = birthdate;
        }
    }
}