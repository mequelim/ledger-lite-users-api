using MediatR;
using Users.Application.Common.Results;

namespace Users.Application.Features.BankAccountFeatures.Get.GetAll
{
    public sealed record GetAllBankAccountsQuery() : IRequest<Result<GetAllBankAccountsResponse>>;
}