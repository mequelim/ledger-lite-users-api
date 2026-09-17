using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities.Enums;

namespace Users.Application.Features.BankAccountFeatures.Create
{
    public sealed record CreateBankAccountCommand(
        Guid UserId,
        string BankName,
        string? Holder,
        string AccountNumber,
        string Agency,
        BankAccountType BankAccountType
    ) : IRequest<Result<CreateBankAccountResponse>>;
}