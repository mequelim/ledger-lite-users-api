using MediatR;
using Users.Application.Common.Results;
using Users.Application.Features.BankAccountFeatures.Get.GetByBankName;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.BankAccountFeatures.Get.GetByUserName
{
    public sealed class GetBankAccountByUserNameHandler(IBankAccountRepository bankAccountRepository) : IRequestHandler<GetBankAccountByUserNameQuery,
        Result<GetBankAccountByUserNameResponse>>
    {
        public async Task<Result<GetBankAccountByUserNameResponse>> Handle(GetBankAccountByUserNameQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<BankAccount> bankAccounts = await bankAccountRepository.GetBankAccountByUserNameAsync(query.UserName, cancellationToken);
            GetBankAccountByUserNameResponse response = new(bankAccounts);

            return Result<GetBankAccountByUserNameResponse>.Success(response);
        }
    }
}