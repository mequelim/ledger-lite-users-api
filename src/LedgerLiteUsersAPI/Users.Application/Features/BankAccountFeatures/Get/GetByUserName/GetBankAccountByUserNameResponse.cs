using Users.Domain.Entities;

namespace Users.Application.Features.BankAccountFeatures.Get.GetByUserName
{
    public sealed record GetBankAccountByUserNameResponse(IEnumerable<BankAccount> BanksAccountsList);
}