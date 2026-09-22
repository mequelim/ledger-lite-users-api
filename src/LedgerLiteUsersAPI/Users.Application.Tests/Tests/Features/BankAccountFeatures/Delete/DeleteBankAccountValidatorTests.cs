using FluentAssertions;
using FluentValidation.Results;
using Users.Application.Features.BankAccountFeatures.Delete;

namespace Users.Application.Tests.Tests.Features.BankAccountFeatures.Delete
{
    public class DeleteBankAccountValidatorTests
    {
        private readonly DeleteBankAccountValidator _accountValidator = new();

        // Methods:
        private static DeleteBankAccountCommand CreateValidCommand()
        {
            return new DeleteBankAccountCommand(
                Id: Guid.NewGuid()
            );
        }

        // Tests:
        [Fact]
        public async Task Validate_ShouldReturnSuccess_WhenCommandIsValid()
        {
            // Arrange:
            DeleteBankAccountCommand command = CreateValidCommand();

            // Act:
            ValidationResult result = await _accountValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task Validate_ShouldReturnValidationError_WhenIdIsEmpty()
        {
            // Arrange:
            DeleteBankAccountCommand command = new DeleteBankAccountCommand(Id: Guid.Empty);

            // Act:
            ValidationResult result = await _accountValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();

            result.Errors.Should().Contain(
                failure =>
                    failure.PropertyName == nameof(command.Id) &&
                    failure.ErrorMessage == "The bank account id is required!"
            );
        }

        [Fact]
        public async Task Validate_ShouldReturnSingleValidationError_WhenIdIsEmpty()
        {
            // Arrange:
            DeleteBankAccountCommand command = new(Guid.Empty);

            // Act:
            ValidationResult result = await _accountValidator.ValidateAsync(command);

            // Assert:
            result.Errors.Should().HaveCount(1);

            result.Errors.First().PropertyName.Should().Be(nameof(command.Id));
            result.Errors.First().ErrorMessage.Should().Be("The bank account id is required!");
        }
    }
}