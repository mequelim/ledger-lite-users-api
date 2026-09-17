using Users.Domain.Entities.Enums;

namespace Users.Application.Features.BankAccountFeatures.Create
{
    public sealed record CreateBankAccountResponse(
        Guid Id,
        Guid UserId,
        string BankName,
        string? Holder,
        string AccountNumber,
        string Agency,
        BankAccountType BankAccountType
    );
}