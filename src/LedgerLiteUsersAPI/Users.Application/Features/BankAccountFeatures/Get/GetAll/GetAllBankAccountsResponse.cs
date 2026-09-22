using Users.Application.DTO;
using Users.Domain.Entities;

namespace Users.Application.Features.BankAccountFeatures.Get.GetAll
{
    public sealed record GetAllBankAccountsResponse(IEnumerable<BankAccount> BankAccountsList);
}