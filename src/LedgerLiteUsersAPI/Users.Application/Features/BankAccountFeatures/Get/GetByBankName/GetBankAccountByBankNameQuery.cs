using MediatR;
using Users.Application.Common.Results;

namespace Users.Application.Features.BankAccountFeatures.Get.GetByBankName
{
    public sealed record GetBankAccountByBankNameQuery(string BankName) : IRequest<Result<GetBankAccountByBankNameResponse>>;
}