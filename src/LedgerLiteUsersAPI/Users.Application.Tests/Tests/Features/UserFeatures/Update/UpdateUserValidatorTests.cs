using FluentAssertions;
using FluentValidation.Results;
using Users.Application.Features.UserFeatures.Update;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Update
{
    public class UpdateUserValidatorTests
    {
        private readonly UpdateUserValidator _userValidator = new();

        [Fact]
        public async Task Validate_ShouldReturnSuccess_WhenCommandIsValid()
        {
            // Arrange:
            UpdateUserCommand command = CreateValidCommand();

            // Act:
            ValidationResult result = await _userValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenUserIdIsEmpty()
        {
            // Arrange:
            UpdateUserCommand command = CreateValidCommand() with
            {
                Id = Guid.Empty
            };

            // Act:
            ValidationResult result = await _userValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();

            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Id) &&
                             failure.ErrorMessage == "The user id is required!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenNameIsEmpty()
        {
            // Arrange:
            UpdateUserCommand command = CreateValidCommand() with
            {
                Name = string.Empty
            };

            // Act:
            ValidationResult result = await _userValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();

            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Name) &&
                             failure.ErrorMessage == "The user name is required!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenSurnameIsEmpty()
        {
            // Arrange:
            UpdateUserCommand command = CreateValidCommand() with
            {
                Surname = string.Empty
            };

            // Act:
            ValidationResult result = await _userValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();

            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Surname) &&
                             failure.ErrorMessage == "The user surname is required!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenBirthdateIsInvalid()
        {
            // Arrange:
            UpdateUserCommand command = CreateValidCommand() with
            {
                Birthdate = DateOnly.FromDateTime(DateTime.Today)
            };

            // Act:
            ValidationResult result = await _userValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();

            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Birthdate) &&
                             failure.ErrorMessage == "The user must be between 18 and 100 years old!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenEmailIsInvalid()
        {
            // Arrange:
            UpdateUserCommand command = CreateValidCommand() with
            {
                Email = "invalid-email"
            };

            // Act:
            ValidationResult result = await _userValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();

            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Email) &&
                             failure.ErrorMessage == "The user e-mail must be a valid e-mail!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenPhoneIsInvalid()
        {
            // Arrange:
            UpdateUserCommand command = CreateValidCommand() with
            {
                Phone = "123"
            };

            // Act:
            ValidationResult result = await _userValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();

            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Phone) &&
                             failure.ErrorMessage == "The user phone must be a valid phone!"
            );
        }

        private static UpdateUserCommand CreateValidCommand()
        {
            return new UpdateUserCommand(
                Guid.NewGuid(),
                "Pedro",
                "Silva",
                DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
                "pedro@email.com",
                "11999999999",
                true
            );
        }
    }
}