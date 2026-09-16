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
        /// Retrieves all bank accounts associated with the specified user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose bank accounts are to be retrieved.</param>
        /// <returns>An asynchronous task that resolves to an enumerable collection of <see cref="BankAccount"/> objects associated with the specified user ID.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided user ID is empty.</exception>
        public async Task<IEnumerable<BankAccount?>> GetBankAccountByUserIdAsync(Guid userId)
        {
            if(userId == Guid.Empty) throw new ArgumentException("User id cannot be empty!", nameof(userId));

            return await databaseContext.BankAccounts
                .AsNoTracking()
                .Where((bankAccount) => bankAccount.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a collection of bank accounts associated with a user whose name matches the specified search term.
        /// </summary>
        /// <param name="userName">The name or part of the name of the user to search for. This can match the user's first name, last name, or full name.</param>
        /// <returns>An asynchronous task resolving to an enumerable collection of <see cref="BankAccount"/> objects that match the specified user name.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided <paramref name="userName"/> is null, empty, or consists only of whitespace.</exception>
        public async Task<IEnumerable<BankAccount>> GetBankAccountByUserNameAsync(string userName)
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
        /// Adds a new bank account to the database.
        /// </summary>
        /// <param name="bankAccount">The bank account object to be added.</param>
        /// <returns>The added <see cref="BankAccount"/> object.</returns>
        public BankAccount Create(BankAccount bankAccount)
        {
            databaseContext.BankAccounts.Add(bankAccount);

            return bankAccount;
        }

        /// <summary>
        /// Updates an existing bank account in the database.
        /// </summary>
        /// <param name="bankAccount">The bank account entity containing the updated data.</param>
        /// <returns>The updated <see cref="BankAccount"/> object.</returns>
        /// <exception cref="InvalidBankAccountAccountNumberException">Thrown when the account number of the bank account is invalid.</exception>
        /// <exception cref="InvalidBankAccountAgencyException">Thrown when the agency of the bank account is invalid.</exception>
        /// <exception cref="InvalidBankNameException">Thrown when the bank name is null, empty, or contains only whitespace.</exception>
        public BankAccount Update(BankAccount bankAccount)
        {
            if(!new BankAccountDataValidator().IsValidAccountNumber(bankAccount.AccountNumber))
            {
                throw new InvalidBankAccountAccountNumberException(bankAccount.AccountNumber);
            }
            if(!new BankAccountDataValidator().IsValidAgency(bankAccount.Agency)) throw new InvalidBankAccountAgencyException(bankAccount.Agency);
            if(string.IsNullOrWhiteSpace(bankAccount.BankName)) throw new InvalidBankNameException(bankAccount.BankName);

            databaseContext.BankAccounts.Update(bankAccount);

            return bankAccount;
        }

        /// <summary>
        /// Deletes a bank account with the specified ID from the database.
        /// </summary>
        /// <param name="bankAccountId">The unique identifier of the bank account to delete.</param>
        /// <returns>An asynchronous task that resolves to the deleted <see cref="BankAccount"/> object.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided bank account ID is empty.</exception>
        /// <exception cref="BankAccountNotFoundException">Thrown when no bank account with the specified ID is found.</exception>
        public async Task<BankAccount> DeleteAsync(Guid bankAccountId)
        {
            if(bankAccountId == Guid.Empty) throw new ArgumentException("Bank account id cannot be empty!", nameof(bankAccountId));

            BankAccount bankAccount = await databaseContext.BankAccounts
                                          .FirstOrDefaultAsync((b) => b.Id == bankAccountId)
                                      ?? throw new BankAccountNotFoundException(nameof(BankAccount.Id), bankAccountId);

            databaseContext.BankAccounts.Remove(bankAccount);

            return bankAccount;
        }
    }
}