using MediatR;
using Users.Application.Common.Results;

namespace Users.Application.Features.BankAccountFeatures.Get.GetByUserId
{
    public sealed record GetBankAccountByUserIdQuery(Guid UserId) : IRequest<Result<GetBankAccountByUserIdResponse>>;
}