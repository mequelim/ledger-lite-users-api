using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.BankAccountFeatures.Get.GetByUserId
{
    public sealed class GetBankAccountByUserIdHandler(IBankAccountRepository bankAccountRepository) : IRequestHandler<GetBankAccountByUserIdQuery, Result<GetBankAccountByUserIdResponse>>
    {
        public async Task<Result<GetBankAccountByUserIdResponse>> Handle(GetBankAccountByUserIdQuery query, CancellationToken cancellationToken)
        {
            if(query.UserId == Guid.Empty) return Result<GetBankAccountByUserIdResponse>.Failure("User not found!");

            IEnumerable<BankAccount> bankAccounts = await bankAccountRepository.GetBankAccountByUserIdAsync(query.UserId, cancellationToken);

            GetBankAccountByUserIdResponse response = new(bankAccounts);

            return Result<GetBankAccountByUserIdResponse>.Success(response);
        }
    }
}