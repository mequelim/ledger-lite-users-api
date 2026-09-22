using FluentValidation;

namespace Users.Application.Features.BankAccountFeatures.Delete
{
    public class DeleteBankAccountValidator : AbstractValidator<DeleteBankAccountCommand>
    {
        public DeleteBankAccountValidator()
        {
            RuleFor((bankAccount) => bankAccount.Id)
                .NotEmpty()
                .WithMessage("The bank account id is required!");
        }
    }
}