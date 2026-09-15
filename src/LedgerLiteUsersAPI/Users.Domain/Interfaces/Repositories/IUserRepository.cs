using Users.Domain.Entities;

namespace Users.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetAllUsersActiveUsersAsync();
        Task<IEnumerable<User>> GetAllUsersInactiveUsersAsync();
        Task<User> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<User>> GetUserByNameOrSurnameAsync(string name);
        Task<IEnumerable<User>> GetUserByFullNameAsync(string fullName);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByPhoneAsync(string phone);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<User> DeleteAsync(Guid userId);
    }
}