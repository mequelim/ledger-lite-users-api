using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.BankAccountFeatures.Get.GetAll
{
    public sealed class GetAllBankAccountsHandler(IBankAccountRepository bankAccountRepository) : IRequestHandler<GetAllBankAccountsQuery, Result<GetAllBankAccountsResponse>>
    {
        public async Task<Result<GetAllBankAccountsResponse>> Handle(GetAllBankAccountsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<BankAccount> bankAccounts = await bankAccountRepository.GetAllAsync(cancellationToken);

            GetAllBankAccountsResponse response = new(bankAccounts);

            return Result<GetAllBankAccountsResponse>.Success(response);
        }
    }
}