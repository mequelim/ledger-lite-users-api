using MediatR;
using Users.Application.Common.Results;

namespace Users.Application.Features.BankAccountFeatures.Get.GetByUserName
{
    public sealed record GetBankAccountByUserNameQuery(string UserName) : IRequest<Result<GetBankAccountByUserNameResponse>>;
}