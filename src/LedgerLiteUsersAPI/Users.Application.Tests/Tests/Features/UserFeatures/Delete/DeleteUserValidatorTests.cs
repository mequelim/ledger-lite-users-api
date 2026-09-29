using FluentAssertions;
using FluentValidation.Results;
using Users.Application.Features.UserFeatures.Delete;

namespace Users.Application.Tests.Tests.Features.UserFeatures.Delete
{
    public class DeleteUserValidatorTests
    {
        private readonly DeleteUserValidator _userValidator = new();

        [Fact]
        public async Task Validate_ShouldReturnSuccess_WhenCommandIsValid()
        {
            // Arrange:
            DeleteUserCommand command = new(Guid.NewGuid());

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
            DeleteUserCommand command = new(Guid.Empty);

            // Act:
            ValidationResult result = await _userValidator.ValidateAsync(command);

            // Assert:
            result.IsValid.Should().BeFalse();

            result.Errors.Should().Contain(
                (failure) => failure.PropertyName == nameof(command.Id) &&
                             failure.ErrorMessage == "The user id is required!"
            );
        }
    }
}