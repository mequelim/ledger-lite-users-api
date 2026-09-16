using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Domain.Exceptions.UserExceptions;
using Users.Domain.Interfaces.Repositories;
using Users.Persistence.Database;

namespace Users.Persistence.Repositories
{
    public class UserRepository(AppDbContext databaseContext) : IUserRepository
    {
        /// <summary>
        /// Retrieves all users from the repository asynchronously.
        /// </summary>
        /// <returns>An enumerable collection of all users.</returns>
        public async Task<IEnumerable<User>> GetAllAsync() => await databaseContext.Users.ToListAsync();

        /// <summary>
        /// Retrieves all active users from the repository asynchronously.
        /// </summary>
        /// <returns>An enumerable collection of active users.</returns>
        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await databaseContext.Users
                .AsNoTracking()
                .Where((user) => user.IsActive == true)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves all inactive users from the repository asynchronously.
        /// </summary>
        /// <returns>An enumerable collection of inactive users.</returns>
        public async Task<IEnumerable<User>> GetInactiveUsersAsync()
        {
            return await databaseContext.Users
                .AsNoTracking()
                .Where((user) => user.IsActive == false)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a user by their unique identifier asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to retrieve.</param>
        /// <returns>The user matching the provided identifier, or null if no user is found.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided userId is an empty GUID.</exception>
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            if(userId == Guid.Empty) throw new ArgumentException("User id cannot be empty!", nameof(userId));

            return await databaseContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync((user) => user.Id == userId);
        }

        /// <summary>
        /// Retrieves a collection of users whose name or surname matches the specified value asynchronously.
        /// </summary>
        /// <param name="name">The string value to search for in the users' name or surname. Cannot be null, empty, or whitespace.</param>
        /// <returns>A collection of users whose name or surname contains the specified value.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided name is null, empty, or consists only of whitespace.</exception>
        public async Task<IEnumerable<User>> GetUserByNameOrSurnameAsync(string name)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("User name cannot be null or empty!", nameof(name));

            string search = name.Trim();

            return await databaseContext.Users
                .AsNoTracking()
                .Where(
                    (user) => user.Name.Contains(search) ||
                              user.Surname.Contains(search)
                )
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a collection of users whose full name matches the specified value asynchronously.
        /// </summary>
        /// <param name="fullName">The full name of the user to search for.</param>
        /// <returns>A collection of users that match the specified full name.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided full name is null, empty, or whitespace.</exception>
        public async Task<IEnumerable<User>> GetUserByFullNameAsync(string fullName)
        {
            if(string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("User name cannot be null or empty!", nameof(fullName));

            string searchTerm = $"%{fullName.Trim()}%";

            return await databaseContext.Users
                .AsNoTracking()
                .Where((user) => EF.Functions.Like(user.Name + " " + user.Surname, searchTerm))
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a user from the repository based on their email address asynchronously.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>The user associated with the specified email address, or null if no user is found.</returns>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            if(string.IsNullOrWhiteSpace(email)) throw new ArgumentException("User email cannot be null or empty!", nameof(email));

            return await databaseContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync((user) => user.Email == email);
        }

        /// <summary>
        /// Retrieves a user from the repository based on their phone number asynchronously.
        /// </summary>
        /// <param name="phone">The phone number of the user to retrieve.</param>
        /// <returns>The user associated with the specified phone number.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided phone number is null or empty.</exception>
        /// <exception cref="InvalidUserPhoneException">Thrown when no user is found with the specified phone number.</exception>
        public async Task<User> GetUserByPhoneAsync(string phone)
        {
            if(string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("User phone cannot be null or empty!", nameof(phone));

            return await databaseContext.Users
                       .AsNoTracking()
                       .SingleOrDefaultAsync((user) => user.Phone == phone)
                   ?? throw new InvalidUserPhoneException(nameof(phone));
        }

        /// <summary>
        /// Asynchronously creates a new user in the repository.
        /// </summary>
        /// <param name="user">The user entity to add to the repository.</param>
        /// <returns>The created user entity.</returns>
        public User Create(User user)
        {
            databaseContext.Users.Add(user);

            return user;
        }

        /// <summary>
        /// Updates an existing user in the repository.
        /// </summary>
        /// <param name="user">The user entity to update.</param>
        /// <returns>The updated user entity.</returns>
        public User Update(User user)
        {
            databaseContext.Users.Update(user);

            return user;
        }

        /// <summary>
        /// Deletes the user with the specified identifier from the repository asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to be deleted.</param>
        /// <returns>The deleted user entity.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided user identifier is empty.</exception>
        /// <exception cref="UserNotFoundException">Thrown when a user with the specified identifier does not exist.</exception>
        public async Task<User> DeleteAsync(Guid userId)
        {
            if(userId == Guid.Empty) throw new ArgumentException("User id cannot be empty!", nameof(userId));

            User user = await databaseContext.Users
                            .FirstOrDefaultAsync((user) => user.Id == userId)
                        ?? throw new UserNotFoundException(nameof(User.Id), userId);

            databaseContext.Users.Remove(user);

            return user;
        }
    }
}