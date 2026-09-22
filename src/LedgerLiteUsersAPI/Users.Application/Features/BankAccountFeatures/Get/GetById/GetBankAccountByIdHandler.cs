using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities;
using Users.Domain.Interfaces.Repositories;

namespace Users.Application.Features.BankAccountFeatures.Get.GetById
{
    public sealed class GetBankAccountByIdHandler(IBankAccountRepository bankAccountRepository) : IRequestHandler<GetBankAccountByIdQuery, Result<GetBankAccountByIdResponse>>
    {
        public async Task<Result<GetBankAccountByIdResponse>> Handle(GetBankAccountByIdQuery query, CancellationToken cancellationToken)
        {
            if(query.Id == Guid.Empty) return Result<GetBankAccountByIdResponse>.Failure("The bank account id cannot be null!");

            BankAccount? bankAccount = await bankAccountRepository.GetBankAccountByIdAsync(query.Id, cancellationToken);

            if(bankAccount is null) return Result<GetBankAccountByIdResponse>.Failure("Bank account not found!");

            GetBankAccountByIdResponse response = new(bankAccount.Id);

            return Result<GetBankAccountByIdResponse>.Success(response);
        }
    }
}