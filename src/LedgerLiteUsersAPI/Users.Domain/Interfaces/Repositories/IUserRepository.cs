using Users.Domain.Entities;

namespace Users.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> GetInactiveUsersAsync();
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<User>> GetUserByNameOrSurnameAsync(string name);
        Task<IEnumerable<User>> GetUserByFullNameAsync(string fullName);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> GetUserByPhoneAsync(string phone);
        User Create(User user);
        User Update(User user);
        Task<User> DeleteAsync(Guid userId);
    }
}