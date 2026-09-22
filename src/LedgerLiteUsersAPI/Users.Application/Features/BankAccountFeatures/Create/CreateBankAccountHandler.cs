using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities;
using Users.Domain.Exceptions.BankAccount;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Features.BankAccountFeatures.Create
{
    public class CreateBankAccountHandler(
        IUserRepository userRepository,
        IBankAccountRepository bankAccountRepository,
        AppDbContext databaseContext
    ) : IRequestHandler<CreateBankAccountCommand, Result<CreateBankAccountResponse>>
    {
        public async Task<Result<CreateBankAccountResponse>> Handle(CreateBankAccountCommand command, CancellationToken cancellationToken)
        {
            User? user = await userRepository.GetUserByIdAsync(command.UserId, cancellationToken);

            if(user is null) return Result<CreateBankAccountResponse>.Failure("User not found!");

            BankAccount bankAccount;

            try
            {
                bankAccount = new BankAccount(
                    command.BankName,
                    command.Holder,
                    command.AccountNumber,
                    command.Agency,
                    command.BankAccountType,
                    command.UserId
                );
            }
            catch(InvalidAccountNumberException exception)
            {
                return Result<CreateBankAccountResponse>.Failure(exception.Message);
            }
            catch(InvalidAgencyException exception)
            {
                return Result<CreateBankAccountResponse>.Failure(exception.Message);
            }

            bankAccountRepository.Create(bankAccount);
            await databaseContext.SaveChangesAsync(cancellationToken);

            CreateBankAccountResponse response = new(
                bankAccount.Id,
                bankAccount.UserId,
                bankAccount.BankName,
                bankAccount.Holder,
                bankAccount.AccountNumber,
                bankAccount.Agency,
                bankAccount.BankAccountType
            );

            return Result<CreateBankAccountResponse>.Success(response);
        }
    }
}