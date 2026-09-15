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
        public async Task<IEnumerable<User>> GetAllUsersActiveUsersAsync()
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
        public async Task<IEnumerable<User>> GetAllUsersInactiveUsersAsync()
        {
            return await databaseContext.Users
                .AsNoTracking()
                .Where((user) => user.IsActive == false)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a user by their unique identifier asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to be retrieved.</param>
        /// <returns>The user associated with the specified identifier.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided userId is an empty Guid.</exception>
        /// <exception cref="UserNotFoundException">Thrown when a user with the specified identifier is not found.</exception>
        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            if(userId == Guid.Empty) throw new ArgumentException("User id cannot be empty!", nameof(userId));

            return await databaseContext.Users
                       .AsNoTracking()
                       .SingleOrDefaultAsync((user) => user.Id == userId)
                   ?? throw new UserNotFoundException(nameof(User.Id), userId);
        }

        /// <summary>
        /// Retrieves users from the repository whose name or surname matches the provided search term asynchronously.
        /// </summary>
        /// <param name="name">The name or surname of the user to search for. It cannot be null or empty.</param>
        /// <returns>An enumerable collection of users that match the specified search criteria.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided name is null or empty.</exception>
        public async Task<IEnumerable<User>> GetUserByNameOrSurnameAsync(string name)
        {
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("User name cannot be null or empty!", nameof(name));

            string searchTerm = $"%{name.Trim()}%";

            return await databaseContext.Users
                .AsNoTracking()
                .Where(
                    (user) => EF.Functions.Like(name, searchTerm) ||
                                     EF.Functions.Like(name, searchTerm)
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
        /// Retrieves a user by their email address asynchronously.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the user with the specified email.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided email is null or empty.</exception>
        /// <exception cref="InvalidUserEmailException">Thrown when no user with the specified email is found.</exception>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            if(string.IsNullOrWhiteSpace(email)) throw new ArgumentException("User email cannot be null or empty!", nameof(email));

            return await databaseContext.Users
                       .AsNoTracking()
                       .SingleOrDefaultAsync((user) => user.Email == email)
                   ?? throw new InvalidUserEmailException(nameof(email));
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
        /// Creates a new user and saves it to the repository asynchronously.
        /// </summary>
        /// <param name="user">The user entity to create.</param>
        /// <returns>The created user entity after it has been saved.</returns>
        public async Task<User> CreateAsync(User user)
        {
            databaseContext.Users.Add(user);
            await databaseContext.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Updates an existing user in the repository asynchronously.
        /// </summary>
        /// <param name="user">The user entity to update.</param>
        /// <returns>The updated user entity.</returns>
        public async Task<User> UpdateAsync(User user)
        {
            databaseContext.Users.Update(user);
            await databaseContext.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Deletes a user from the repository by their unique identifier asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to be deleted.</param>
        /// <returns>The user that was deleted.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided user ID is empty.</exception>
        /// <exception cref="UserNotFoundException">Thrown when a user with the specified ID is not found.</exception>
        public async Task<User> DeleteAsync(Guid userId)
        {
            if(userId == Guid.Empty) throw new ArgumentException("User id cannot be empty!", nameof(userId));

            User user = await databaseContext.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync((user) => user.Id == userId)
                        ?? throw new UserNotFoundException(nameof(User.Id), userId);

            databaseContext.Users.Remove(user);
            await databaseContext.SaveChangesAsync();

            return user;
        }
    }
}