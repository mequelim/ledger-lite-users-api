using Users.Domain.Entities;

namespace Users.Domain.Interfaces.Repositories
{
    public interface IBankAccountRepository
    {
        Task<IEnumerable<BankAccount>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<BankAccount?> GetBankAccountByIdAsync(Guid bankAccountId, CancellationToken cancellationToken = default);
        Task<IEnumerable<BankAccount>> GetBankAccountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<BankAccount>> GetBankAccountByUserNameAsync(string userName, CancellationToken cancellationToken = default);
        Task<IEnumerable<BankAccount>> GetBankAccountByBankNameAsync(string bankName, CancellationToken cancellationToken = default);
        BankAccount Create(BankAccount bankAccount);
        BankAccount Update(BankAccount bankAccount);
        Task<BankAccount> DeleteAsync(Guid bankAccountId, CancellationToken cancellationToken = default);
    }
}