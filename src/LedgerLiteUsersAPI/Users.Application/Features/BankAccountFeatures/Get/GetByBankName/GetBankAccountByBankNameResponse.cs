using Users.Domain.Entities;

namespace Users.Application.Features.BankAccountFeatures.Get.GetByBankName
{
    public sealed record GetBankAccountByBankNameResponse(IEnumerable<BankAccount> BanksAccountsList);
}