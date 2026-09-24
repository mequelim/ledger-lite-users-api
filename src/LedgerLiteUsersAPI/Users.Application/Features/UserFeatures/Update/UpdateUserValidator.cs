using FluentValidation;
using Users.Domain.Validators;

namespace Users.Application.Features.UserFeatures.Update
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
            RuleFor((user) => user.Id)
                .NotEmpty()
                .WithMessage("The user id is required!");

            RuleFor((user) => user.Name)
                .NotEmpty()
                .WithMessage("The user name is required!")
                .MaximumLength(100)
                .WithMessage("The user name must not exceed 100 character!");

            RuleFor((user) => user.Surname)
                .NotEmpty()
                .WithMessage("The user surname is required!")
                .MaximumLength(100)
                .WithMessage("The user surname must not exceed 100 character!");

            RuleFor((user) => user.Birthdate)
                .NotEmpty()
                .WithMessage("The user birthdate is required!")
                .Must(new UserDataValidator().IsValidBirthdate)
                .WithMessage("The user must be between 18 and 100 years old!");

            RuleFor((user) => user.Email)
                .NotEmpty()
                .WithMessage("The user e-mail is required!")
                .MaximumLength(150)
                .WithMessage("The user e-mail must not exceed 150 character!")
                .Must(new UserDataValidator().IsValidEmail)
                .WithMessage("The user e-mail must be a valid e-mail!");

            RuleFor((user) => user.Phone)
                .NotEmpty()
                .WithMessage("The user phone is required!")
                .MaximumLength(20)
                .WithMessage("The user phone must not exceed 20 character!")
                .Must(new UserDataValidator().IsValidPhone)
                .WithMessage("The user phone must be a valid phone!");
        }
    }
}