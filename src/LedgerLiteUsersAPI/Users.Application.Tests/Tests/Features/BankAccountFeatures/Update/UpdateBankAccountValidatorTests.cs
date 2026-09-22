using FluentAssertions;
using FluentValidation.Results;
using Users.Application.Features.BankAccountFeatures.Update;
using Users.Domain.Entities.Enums;

namespace Users.Application.Tests.Tests.Features.BankAccountFeatures.Update
{
    public class UpdateBankAccountValidatorTests
    {
        private readonly UpdateBankAccountValidator _accountValidator = new();

        // Methods:
        private static UpdateBankAccountCommand CreateValidCommand()
        {
            return new UpdateBankAccountCommand(
                Id: Guid.NewGuid(),
                BankName: "Nubank",
                Holder: null,
                AccountNumber: "1234567890",
                Agency: "0001",
                BankAccountType: BankAccountType.Checking
            );
        }

        // Tests:
        [Fact]
        public async Task Validate_ShouldReturnSuccess_WhenCommandIsValid()
        {
            // Arrange:
            UpdateBankAccountCommand command = CreateValidCommand();

            // Act:
            ValidationResult result = await _accountValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenAccountNumberIsEmpty()
        {
            // Arrange:
            UpdateBankAccountCommand command = CreateValidCommand() with
            {
                AccountNumber = string.Empty
            };

            // Act:
            ValidationResult result = await _accountValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.AccountNumber) &&
                             failure.ErrorMessage == "The bank account is required!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenAccountNumberExceedsMaximumLength()
        {
            // Arrange:
            UpdateBankAccountCommand command = CreateValidCommand() with
            {
                AccountNumber = new string('1', 21)
            };

            // Act:
            ValidationResult result = await _accountValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.AccountNumber) &&
                             failure.ErrorMessage == "The bank account must not exceed 20 characters!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenAgencyIsEmpty()
        {
            // Arrange:
            UpdateBankAccountCommand command = CreateValidCommand() with
            {
                Agency = string.Empty
            };

            // Act:
            ValidationResult result = await _accountValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Agency) &&
                             failure.ErrorMessage == "The agency is required!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenAgencyExceedsMaximumLength()
        {
            // Arrange:
            UpdateBankAccountCommand command = CreateValidCommand() with
            {
                Agency = new string('1', 13)
            };

            // Act:
            ValidationResult result = await _accountValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Agency) &&
                             failure.ErrorMessage == "The agency must not exceed 12 characters!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenBankAccountTypeIsInvalid()
        {
            // Arrange:
            UpdateBankAccountCommand command = CreateValidCommand() with
            {
                BankAccountType = (BankAccountType)999
            };

            // Act:
            ValidationResult result = await _accountValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.BankAccountType) &&
                             failure.ErrorMessage == "You need to select a bank account type!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenBankNameIsEmpty()
        {
            // Arrange:
            UpdateBankAccountCommand command = CreateValidCommand() with
            {
                BankName = string.Empty
            };

            // Act:
            ValidationResult result = await _accountValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.BankName) &&
                             failure.ErrorMessage == "The bank name is required!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenBankNameExceedsMaximumLength()
        {
            // Arrange:
            UpdateBankAccountCommand command = CreateValidCommand() with
            {
                BankName = new string('A', 101)
            };

            // Act:
            ValidationResult result = await _accountValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.BankName) &&
                             failure.ErrorMessage == "The bank name must not exceed 100 characters!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnSuccess_WhenHolderIsNull()
        {
            // Arrange:
            UpdateBankAccountCommand command = CreateValidCommand() with
            {
                Holder = null
            };

            // Act:
            ValidationResult result = await _accountValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenHolderExceedsMaximumLength()
        {
            // Arrange:
            UpdateBankAccountCommand command = CreateValidCommand() with
            {
                Holder = new string('A', 201)
            };

            // Act:
            ValidationResult result = await _accountValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Holder) &&
                             failure.ErrorMessage == "The holder name must not exceed 200 characters!"
            );
        }
    }
}