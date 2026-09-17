namespace Users.Application.Common.Results
{
    public sealed class Result<T> : Results
    {
        public T Value { get; }

        // Constructor:
        private Result(bool isSuccess, T value, string errorMessage) : base(isSuccess, errorMessage) => Value = value;

        // Methods:
        public static Result<T> Success(T value) => new(true, value, string.Empty);
        public static Result<T> Failure(string errorMessage) => new(false, default!, errorMessage);
    }
}