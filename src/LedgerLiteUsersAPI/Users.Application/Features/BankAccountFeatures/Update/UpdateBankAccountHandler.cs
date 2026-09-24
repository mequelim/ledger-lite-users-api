using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities;
using Users.Domain.Exceptions.BankAccount;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Features.BankAccountFeatures.Update
{
    public sealed class UpdateBankAccountHandler(
        IBankAccountRepository bankAccountRepository,
        AppDbContext databaseContext
    ) : IRequestHandler<UpdateBankAccountCommand, Result<UpdateBankAccountResponse>>
    {
        public async Task<Result<UpdateBankAccountResponse>> Handle(UpdateBankAccountCommand command, CancellationToken cancellationToken)
        {
            BankAccount? bankAccount = await bankAccountRepository.GetBankAccountByIdAsync(command.Id, cancellationToken);

            if(bankAccount is null) return Result<UpdateBankAccountResponse>.Failure("Bank account not found!");

            bankAccount.BankName = command.BankName;
            bankAccount.Holder = command.Holder;
            bankAccount.AccountNumber = command.AccountNumber;
            bankAccount.Agency = command.Agency;
            bankAccount.BankAccountType = command.BankAccountType;

            bankAccountRepository.Update(bankAccount);
            await databaseContext.SaveChangesAsync(cancellationToken);

            UpdateBankAccountResponse response = new(
                bankAccount.Id,
                bankAccount.UserId,
                bankAccount.BankName,
                bankAccount.Holder,
                bankAccount.AccountNumber,
                bankAccount.Agency,
                bankAccount.BankAccountType
            );

            return Result<UpdateBankAccountResponse>.Success(response);
        }
    }
}