using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.BankAccountFeatures.Get.GetByBankName
{
    public sealed class GetBankAccountByBankNameHandler(IBankAccountRepository bankAccountRepository) : IRequestHandler<GetBankAccountByBankNameQuery, Result<GetBankAccountByBankNameResponse>>
    {
        public async Task<Result<GetBankAccountByBankNameResponse>> Handle(GetBankAccountByBankNameQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<BankAccount> bankAccounts = await bankAccountRepository.GetBankAccountByBankNameAsync(query.BankName, cancellationToken);

            GetBankAccountByBankNameResponse response = new(bankAccounts);

            return Result<GetBankAccountByBankNameResponse>.Success(response);
        }
    }
}