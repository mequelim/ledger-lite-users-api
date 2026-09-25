using Users.Domain.Entities;

namespace Users.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAllActiveUsersAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAllInactiveUsersAsync(CancellationToken cancellationToken = default);
        Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetUserByNameOrSurnameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetUserByFullNameAsync(string fullName, CancellationToken cancellationToken = default);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetUserByPhoneAsync(string phone, CancellationToken cancellationToken = default);
        User Create(User user);
        User Update(User user);
        Task<User> DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}