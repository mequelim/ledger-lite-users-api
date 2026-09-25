using FluentValidation;

namespace Users.Application.Features.UserFeatures.Delete
{
    public sealed class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserValidator()
        {
            RuleFor((user) => user.Id)
                .NotEmpty()
                .WithMessage("The user id is required!");
        }
    }
}