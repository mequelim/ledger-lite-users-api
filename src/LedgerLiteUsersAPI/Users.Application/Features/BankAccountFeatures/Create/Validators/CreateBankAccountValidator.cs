using FluentValidation;

namespace Users.Application.Features.BankAccountFeatures.Create.Validators
{
    public class CreateBankAccountValidator : AbstractValidator<CreateBankAccountCommand>
    {
        public CreateBankAccountValidator()
        {
            RuleFor((bankAccount) => bankAccount.AccountNumber)
                .NotEmpty()
                .WithMessage("The bank account is required!")
                .MaximumLength(20)
                .WithMessage("The bank account must not exceed 20 characters!");

            RuleFor((bankAccount) => bankAccount.Agency)
                .NotEmpty()
                .WithMessage("The agency is required!")
                .MaximumLength(12)
                .WithMessage("The agency must not exceed 12 characters!");

            RuleFor((bankAccount) => bankAccount.BankAccountType)
                .IsInEnum()
                .WithMessage("You need to select a bank account type!");

            RuleFor((bankAccount) => bankAccount.BankName)
                .NotEmpty()
                .WithMessage("The bank name is required!")
                .MaximumLength(100)
                .WithMessage("The bank name must not exceed 100 characters!");

            // Fields that can be null:
            RuleFor((bankAccount) => bankAccount.Holder)
                .MaximumLength(200)
                .WithMessage("The holder name must not exceed 200 characters!");

            // Foreign Key (FK):
            RuleFor((bankAccount) => bankAccount.UserId)
                .NotEmpty()
                .WithMessage("The user id is required!");
        }
    }
}