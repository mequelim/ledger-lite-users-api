using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Domain.Exceptions.BankAccount;
using Users.Domain.Interfaces.Repositories;
using Users.Domain.Validators;
using Users.Persistence.Database;

namespace Users.Persistence.Repositories
{
    public class BankAccountRepository(AppDbContext databaseContext) : IBankAccountRepository
    {
        public async Task<IEnumerable<BankAccount>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await databaseContext.BankAccounts.ToListAsync(cancellationToken);
        }

        public async Task<BankAccount> GetBankAccountByIdAsync(Guid bankAccountId, CancellationToken cancellationToken)
        {
            if(bankAccountId == Guid.Empty) throw new ArgumentException("Bank account id cannot be empty!", nameof(bankAccountId));

            return await databaseContext.BankAccounts
                       .AsNoTracking()
                       .SingleOrDefaultAsync(
                           (bankAccount) => bankAccount.Id == bankAccountId,
                           cancellationToken
                       )
                   ?? throw new BankAccountNotFoundException(nameof(BankAccount.Id), bankAccountId);
        }

        public async Task<IEnumerable<BankAccount?>> GetBankAccountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            if(userId == Guid.Empty) throw new ArgumentException("User id cannot be empty!", nameof(userId));

            return await databaseContext.BankAccounts
                .AsNoTracking()
                .Where((bankAccount) => bankAccount.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BankAccount>> GetBankAccountByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("User name cannot be null or empty!", nameof(userName));

            string searchTerm = $"%{userName.Trim()}%";

            return await databaseContext.BankAccounts
                .AsNoTracking()
                .Include(b => b.User)
                .Where(
                        (bankAccount) => EF.Functions.Like(bankAccount.User.Name, searchTerm) ||
                                         EF.Functions.Like(bankAccount.User.Surname, searchTerm) ||
                                         EF.Functions.Like(bankAccount.User.Name + " " + bankAccount.User.Surname, searchTerm)
                )
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<BankAccount>> GetBankAccountByBankNameAsync(string bankName, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(bankName)) throw new InvalidBankNameException(bankName);

            return await databaseContext.BankAccounts
                .AsNoTracking()
                .Where((bankAccount) => bankAccount.BankName == bankName)
                .ToListAsync(cancellationToken);
        }

        public BankAccount Create(BankAccount bankAccount)
        {
            databaseContext.BankAccounts.Add(bankAccount);

            return bankAccount;
        }

        public BankAccount Update(BankAccount bankAccount)
        {
            if(!new BankAccountDataValidator().IsValidAccountNumber(bankAccount.AccountNumber))
            {
                throw new InvalidAccountNumberException(bankAccount.AccountNumber);
            }
            if(!new BankAccountDataValidator().IsValidAgency(bankAccount.Agency)) throw new InvalidAgencyException(bankAccount.Agency);
            if(string.IsNullOrWhiteSpace(bankAccount.BankName)) throw new InvalidBankNameException(bankAccount.BankName);

            databaseContext.BankAccounts.Update(bankAccount);

            return bankAccount;
        }

        public async Task<BankAccount> DeleteAsync(Guid bankAccountId, CancellationToken cancellationToken)
        {
            if(bankAccountId == Guid.Empty) throw new ArgumentException("Bank account id cannot be empty!", nameof(bankAccountId));

            BankAccount bankAccount = await databaseContext.BankAccounts
                                          .FirstOrDefaultAsync(
                                              (b) => b.Id == bankAccountId,
                                              cancellationToken
                                          )
                                      ?? throw new BankAccountNotFoundException(nameof(BankAccount.Id), bankAccountId);

            databaseContext.BankAccounts.Remove(bankAccount);

            return bankAccount;
        }
    }
}