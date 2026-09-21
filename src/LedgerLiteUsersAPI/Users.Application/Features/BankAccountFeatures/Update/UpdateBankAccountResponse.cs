using Users.Domain.Entities.Enums;

namespace Users.Application.Features.BankAccountFeatures.Update
{
    public sealed record UpdateBankAccountResponse(
        Guid Id,
        Guid UserId,
        string BankName,
        string? Holder,
        string AccountNumber,
        string Agency,
        BankAccountType BankAccountType
    );
}