using Users.Domain.Entities;

namespace Users.Domain.Interfaces.Repositories
{
    public interface IBankAccountRepository
    {
        Task<IEnumerable<BankAccount>> GetAllAsync();
        Task<BankAccount> GetBankAccountByIdAsync(Guid bankAccountId);
        Task<IEnumerable<BankAccount?>> GetBankAccountByUserIdAsync(Guid userId);
        Task<IEnumerable<BankAccount>> GetBankAccountByUserNameAsync(string userName);
        Task<IEnumerable<BankAccount>> GetBankAccountByBankNameAsync(string bankName);
        BankAccount Create(BankAccount bankAccount);
        BankAccount Update(BankAccount bankAccount);
        Task<BankAccount> DeleteAsync(Guid bankAccountId);
    }
}