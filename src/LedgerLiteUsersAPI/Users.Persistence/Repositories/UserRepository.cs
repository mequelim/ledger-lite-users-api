using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Domain.Exceptions.UserExceptions;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Persistence.Repositories
{
    public class UserRepository(AppDbContext databaseContext) : IUserRepository
    {
        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await databaseContext.Users
                .AsNoTracking()
                .Include((user) => user.BankAccounts)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllActiveUsersAsync(CancellationToken cancellationToken)
        {
            return await databaseContext.Users
                .AsNoTracking()
                .Include((user) => user.BankAccounts)
                .Where((user) => user.IsActive == true)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllInactiveUsersAsync(CancellationToken cancellationToken)
        {
            return await databaseContext.Users
                .AsNoTracking()
                .Include((user) => user.BankAccounts)
                .Where((user) => user.IsActive == false)
                .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            if(userId == Guid.Empty) throw new ArgumentException("User id cannot be empty!", nameof(userId));

            return await databaseContext.Users
                .AsNoTracking()
                .Include((user) => user.BankAccounts)
                .SingleOrDefaultAsync(
                    (user) => user.Id == userId,
                    cancellationToken
                );
        }

        public async Task<IEnumerable<User>> GetUserByNameOrSurnameAsync(string name, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("User name cannot be null or empty!", nameof(name));

            string search = name.Trim();

            return await databaseContext.Users
                .AsNoTracking()
                .Include((user) => user.BankAccounts)
                .Where((user) => user.Name.Contains(search) || user.Surname.Contains(search))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetUserByFullNameAsync(string fullName, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("User name cannot be null or empty!", nameof(fullName));

            string searchTerm = $"%{fullName.Trim()}%";

            return await databaseContext.Users
                .AsNoTracking()
                .Include((user) => user.BankAccounts)
                .Where(
                    (user) => EF.Functions.Like(
                        user.Name + " " + user.Surname,
                        searchTerm
                    )
                )
                .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(email)) throw new ArgumentException("User email cannot be null or empty!", nameof(email));

            return await databaseContext.Users
                .AsNoTracking()
                .Include((user) => user.BankAccounts)
                .SingleOrDefaultAsync(
                    (user) => user.Email == email,
                    cancellationToken
                );
        }

        public async Task<User?> GetUserByPhoneAsync(string phone, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("User phone cannot be null or empty!", nameof(phone));

            return await databaseContext.Users
                .AsNoTracking()
                .Include((user) => user.BankAccounts)
                .SingleOrDefaultAsync(
                    (user) => user.Phone == phone,
                    cancellationToken
                );
        }

        public User Create(User user)
        {
            databaseContext.Users.Add(user);

            return user;
        }

        public User Update(User user)
        {
            databaseContext.Users.Update(user);

            return user;
        }

        public async Task<User> DeleteAsync(Guid userId, CancellationToken cancellationToken)
        {
            if(userId == Guid.Empty) throw new ArgumentException("User id cannot be empty!", nameof(userId));

            User user = await databaseContext.Users
                            .FirstOrDefaultAsync(
                                (user) => user.Id == userId,
                                cancellationToken
                            )
                        ?? throw new UserNotFoundException(nameof(User.Id), userId);

            databaseContext.Users.Remove(user);

            return user;
        }
    }
}