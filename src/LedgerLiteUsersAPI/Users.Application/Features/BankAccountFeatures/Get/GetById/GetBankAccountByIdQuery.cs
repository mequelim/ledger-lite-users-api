using MediatR;
using Users.Application.Common.Results;

namespace Users.Application.Features.BankAccountFeatures.Get.GetById
{
    public sealed record GetBankAccountByIdQuery(Guid Id) : IRequest<Result<GetBankAccountByIdResponse>>;
}