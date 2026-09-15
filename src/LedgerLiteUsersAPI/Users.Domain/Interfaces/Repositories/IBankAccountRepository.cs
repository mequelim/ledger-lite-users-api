using Users.Domain.Entities;

namespace Users.Domain.Interfaces.Repositories
{
    public interface IBankAccountRepository
    {
        Task<IEnumerable<BankAccount>> GetAllAsync();
        Task<BankAccount> GetBankAccountByIdAsync(Guid bankAccountId);
        Task<IEnumerable<BankAccount>> GetBankAccountByUserIdAsync(Guid userId);
        Task<IEnumerable<BankAccount>> GetBankAccountByUserNameAsync(string userName);
        Task<IEnumerable<BankAccount>> GetBankAccountByBankNameAsync(string bankName);
        Task<BankAccount> CreateAsync(BankAccount bankAccount);
        Task<BankAccount> UpdateAsync(BankAccount bankAccount);
        Task<BankAccount> DeleteAsync(Guid bankAccountId);
    }
}