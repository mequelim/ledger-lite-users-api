using FluentAssertions;
using FluentValidation.Results;
using Users.Application.Features.UserFeatures.Create;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Create
{
    public class CreateUserValidatorTests
    {
        private readonly CreateUserValidator _userValidator = new();

        private static CreateUserCommand CreateValidCommand()
        {
            return new CreateUserCommand(
                Name: "Pedro",
                Surname: "Henrique",
                Birthdate: DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                Email: "pedro@email.com",
                Phone: "11999999999",
                IsActive: true
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnSuccess_WhenCommandIsValid()
        {
            CreateUserCommand command = CreateValidCommand();
            ValidationResult result = await _userValidator.ValidateAsync(command);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenNameIsEmpty()
        {
            CreateUserCommand command = CreateValidCommand() with
            {
                Name = string.Empty
            };

            ValidationResult result = await _userValidator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Name) &&
                             failure.ErrorMessage == "The user name is required!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenNameExceedsMaximumLength()
        {
            CreateUserCommand command = CreateValidCommand() with
            {
                Name = new string('A', 101)
            };

            ValidationResult result = await _userValidator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Name) &&
                             failure.ErrorMessage == "The user name must not exceed 100 character!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenSurnameIsEmpty()
        {
            CreateUserCommand command = CreateValidCommand() with
            {
                Surname = string.Empty
            };

            ValidationResult result = await _userValidator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Surname) &&
                             failure.ErrorMessage == "The user surname is required!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenSurnameExceedsMaximumLength()
        {
            CreateUserCommand command = CreateValidCommand() with
            {
                Surname = new string('A', 101)
            };

            ValidationResult result = await _userValidator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Surname) &&
                             failure.ErrorMessage == "The user surname must not exceed 100 character!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenBirthdateIsEmpty()
        {
            CreateUserCommand command = CreateValidCommand() with
            {
                Birthdate = default
            };

            ValidationResult result = await _userValidator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Birthdate) &&
                             failure.ErrorMessage == "The user birthdate is required!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenBirthdateIsInvalid()
        {
            CreateUserCommand command = CreateValidCommand() with
            {
                Birthdate = DateOnly.FromDateTime(DateTime.Today.AddYears(-10))
            };

            ValidationResult result = await _userValidator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Birthdate) &&
                             failure.ErrorMessage == "The user must be between 18 and 100 years old!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenEmailIsEmpty()
        {
            CreateUserCommand command = CreateValidCommand() with
            {
                Email = string.Empty
            };

            ValidationResult result = await _userValidator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Email) &&
                             failure.ErrorMessage == "The user e-mail is required!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenEmailExceedsMaximumLength()
        {
            CreateUserCommand command = CreateValidCommand() with
            {
                Email = new string('A', 151)
            };

            ValidationResult result = await _userValidator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Email) &&
                             failure.ErrorMessage == "The user e-mail must not exceed 150 character!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenEmailFormatIsInvalid()
        {
            CreateUserCommand command = CreateValidCommand() with
            {
                Email = "email-invalido"
            };

            ValidationResult result = await _userValidator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Email) &&
                             failure.ErrorMessage == "The user e-mail must be a valid e-mail!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenPhoneIsEmpty()
        {
            CreateUserCommand command = CreateValidCommand() with
            {
                Phone = string.Empty
            };

            ValidationResult result = await _userValidator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Phone) &&
                             failure.ErrorMessage == "The user phone is required!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenPhoneExceedsMaximumLength()
        {
            CreateUserCommand command = CreateValidCommand() with
            {
                Phone = new string('1', 21)
            };

            ValidationResult result = await _userValidator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Phone) &&
                             failure.ErrorMessage == "The user phone must not exceed 20 character!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenPhoneFormatIsInvalid()
        {
            CreateUserCommand command = CreateValidCommand() with
            {
                Phone = "12345"
            };

            ValidationResult result = await _userValidator.ValidateAsync(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Phone) &&
                             failure.ErrorMessage == "The user phone must be a valid phone!"
            );
        }
    }
}