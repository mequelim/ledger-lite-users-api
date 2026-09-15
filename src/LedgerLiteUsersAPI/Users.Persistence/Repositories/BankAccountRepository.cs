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
        /// <summary>
        /// Retrieves all bank accounts from the database.
        /// </summary>
        /// <returns>An asynchronous task that resolves to an enumerable collection of <see cref="BankAccount"/> objects.</returns>
        public async Task<IEnumerable<BankAccount>> GetAllAsync() => await databaseContext.BankAccounts.ToListAsync();

        /// <summary>
        /// Retrieves a bank account from the database by its unique identifier.
        /// </summary>
        /// <param name="bankAccountId">The unique identifier of the bank account to retrieve.</param>
        /// <returns>An asynchronous task that resolves to the requested <see cref="BankAccount"/> object if found.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided bank account ID is empty.</exception>
        /// <exception cref="BankAccountNotFoundException">Thrown when a bank account with the specified ID cannot be found.</exception>
        public async Task<BankAccount> GetBankAccountByIdAsync(Guid bankAccountId)
        {
            if(bankAccountId == Guid.Empty) throw new ArgumentException("Bank account id cannot be empty!", nameof(bankAccountId));

            return await databaseContext.BankAccounts
                       .AsNoTracking()
                       .SingleOrDefaultAsync((bankAccount) => bankAccount.Id == bankAccountId)
                   ?? throw new BankAccountNotFoundException(nameof(BankAccount.Id), bankAccountId);
        }

        /// <summary>
        /// Retrieves all bank accounts associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose bank accounts are to be retrieved.</param>
        /// <returns>An asynchronous task that resolves to an enumerable collection of <see cref="BankAccount"/> objects associated with the specified user.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided user ID is empty.</exception>
        public async Task<IEnumerable<BankAccount>> GetBankAccountByUserIdAsync(Guid userId)
        {
            if(userId == Guid.Empty) throw new ArgumentException("User id cannot be empty!", nameof(userId));

            return await databaseContext.BankAccounts
                .AsNoTracking()
                .Where((bankAccount) => bankAccount.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all bank accounts associated with a user whose name, surname, or full name matches the given user name.
        /// </summary>
        /// <param name="userName">The name or surname of the user to search for.</param>
        /// <returns>An asynchronous task that resolves to an enumerable collection of <see cref="BankAccount"/> objects.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided <paramref name="userName"/> is null, empty, or whitespace.</exception>
        public async Task<IEnumerable<BankAccount>> GetBankAccountByUserNameAsync(string userName)
        {
            if(string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("User name cannot be null or empty!", nameof(userName));

            string searchTerm = $"%{userName.Trim()}%";

            return await databaseContext.BankAccounts
                .AsNoTracking()
                .Where(
                    (bankAccount) => EF.Functions.ILike(bankAccount.User.Name, searchTerm) ||
                                     EF.Functions.ILike(bankAccount.User.Surname, searchTerm) ||
                                     EF.Functions.ILike($"{bankAccount.User.Name} {bankAccount.User.Surname}", searchTerm)
                 )
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all bank accounts associated with the specified bank name.
        /// </summary>
        /// <param name="bankName">The name of the bank whose accounts are to be retrieved.</param>
        /// <returns>An asynchronous task that resolves to an enumerable collection of <see cref="BankAccount"/> objects associated with the specified bank name.</returns>
        /// <exception cref="InvalidBankNameException">Thrown when the provided bank name is null, empty, or consists only of whitespace.</exception>
        public async Task<IEnumerable<BankAccount>> GetBankAccountByBankNameAsync(string bankName)
        {
            if(string.IsNullOrWhiteSpace(bankName)) throw new InvalidBankNameException(bankName);

            return await databaseContext.BankAccounts
                .AsNoTracking()
                .Where((bankAccount) => bankAccount.BankName == bankName)
                .ToListAsync();
        }

        /// <summary>
        /// Creates a new bank account in the database.
        /// </summary>
        /// <param name="bankAccount">The <see cref="BankAccount"/> object to be added to the database.</param>
        /// <returns>An asynchronous task that resolves to the created <see cref="BankAccount"/> object.</returns>
        public async Task<BankAccount> CreateAsync(BankAccount bankAccount)
        {
            databaseContext.BankAccounts.Add(bankAccount);
            await databaseContext.SaveChangesAsync();

            return bankAccount;
        }

        /// <summary>
        /// Updates an existing bank account in the database.
        /// </summary>
        /// <param name="bankAccount">The <see cref="BankAccount"/> object to update.</param>
        /// <returns>An asynchronous task that resolves to the updated <see cref="BankAccount"/> object.</returns>
        public async Task<BankAccount> UpdateAsync(BankAccount bankAccount)
        {
            if(!new BankAccountDataValidator().IsValidAccountNumber(bankAccount.AccountNumber)) throw new InvalidBankAccountAccountNumberException(bankAccount.AccountNumber);
            if(!new BankAccountDataValidator().IsValidAgency(bankAccount.Agency)) throw new InvalidBankAccountAgencyException(bankAccount.Agency);
            if(string.IsNullOrWhiteSpace(bankAccount.BankName)) throw new InvalidBankNameException(bankAccount.BankName);

            databaseContext.BankAccounts.Update(bankAccount);
            await databaseContext.SaveChangesAsync();

            return bankAccount;
        }

        /// <summary>
        /// Deletes a bank account from the database based on the specified identifier.
        /// </summary>
        /// <param name="bankAccountId">The unique identifier of the bank account to be deleted.</param>
        /// <returns>An asynchronous task that resolves to the deleted <see cref="BankAccount"/> object.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided bank account ID is empty.</exception>
        /// <exception cref="BankAccountNotFoundException">Thrown when a bank account with the specified ID cannot be found.</exception>
        public async Task<BankAccount> DeleteAsync(Guid bankAccountId)
        {
            if(bankAccountId == Guid.Empty) throw new ArgumentException("Bank account id cannot be empty!", nameof(bankAccountId));

            BankAccount bankAccount = await databaseContext.BankAccounts
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync((b) => b.Id == bankAccountId)
                                      ?? throw new BankAccountNotFoundException(nameof(BankAccount.Id), bankAccountId);

            databaseContext.BankAccounts.Remove(bankAccount);
            await databaseContext.SaveChangesAsync();

            return bankAccount;
        }
    }
}