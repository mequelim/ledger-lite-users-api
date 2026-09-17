using Users.Domain.Entities;
using Users.Domain.Entities.Enums;

namespace Users.Application.DTO
{
    public sealed record BankAccountDto(
        Guid Id,
        string BankName,
        string? Holder,
        string AccountNumber,
        string Agency,
        BankAccountType BankAccountType,
        Guid UserId
    )
    {
        public static BankAccountDto FromEntity(BankAccount bankAccount) => new(
            Id: bankAccount.Id,
            BankName: bankAccount.BankName,
            Holder: bankAccount.Holder,
            AccountNumber: bankAccount.AccountNumber,
            Agency: bankAccount.Agency,
            BankAccountType: bankAccount.BankAccountType,
            UserId: bankAccount.UserId
        );
    }
}