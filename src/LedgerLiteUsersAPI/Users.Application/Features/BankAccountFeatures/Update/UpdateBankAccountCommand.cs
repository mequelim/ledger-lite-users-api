using MediatR;
using Users.Application.Common.Results;
using Users.Domain.Entities.Enums;

namespace Users.Application.Features.BankAccountFeatures.Update
{
    public sealed record UpdateBankAccountCommand(
        Guid Id,
        string BankName,
        string? Holder,
        string AccountNumber,
        string Agency,
        BankAccountType BankAccountType
    ) : IRequest<Result<UpdateBankAccountResponse>>;
}