using Users.Domain.Entities;

namespace Users.Application.Features.BankAccountFeatures.Get.GetByUserId
{
    public sealed record GetBankAccountByUserIdResponse(IEnumerable<BankAccount> BanksAccountsList);
}