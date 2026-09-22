using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Exceptions.BankAccount;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Application.Features.BankAccountFeatures.Delete
{
    public sealed class DeleteBankAccountHandler(
        IBankAccountRepository bankAccountRepository,
        AppDbContext databaseContext
    ) : IRequestHandler<DeleteBankAccountCommand, Result<DeleteBankAccountResponse>>
    {
        public async Task<Result<DeleteBankAccountResponse>> Handle(DeleteBankAccountCommand command, CancellationToken cancellationToken)
        {
            try
            {
                await bankAccountRepository.DeleteAsync(command.Id, cancellationToken);
                await databaseContext.SaveChangesAsync(cancellationToken);
            }
            catch(BankAccountNotFoundException exception)
            {
                return Result<DeleteBankAccountResponse>.Failure(exception.Message);
            }

            DeleteBankAccountResponse response = new(command.Id);

            return Result<DeleteBankAccountResponse>.Success(response);
        }
    }
}