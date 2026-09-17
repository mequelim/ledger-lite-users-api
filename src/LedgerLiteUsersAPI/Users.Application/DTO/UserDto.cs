using Users.Domain.Entities;

namespace Users.Application.DTO
{
    public sealed record UserDto(
        Guid Id,
        string Name,
        string Surname,
        DateOnly Birthdate,
        string Email,
        string Phone,
        bool IsActive,
        List<BankAccountDto> BankAccounts
    )
    {
        public static UserDto FromEntity(User user) => new(
            Id: user.Id,
            Name: user.Name,
            Surname: user.Surname,
            Birthdate: user.Birthdate,
            Email: user.Email,
            Phone: user.Phone,
            IsActive: user.IsActive,
            BankAccounts: user.BankAccounts.Select(BankAccountDto.FromEntity).ToList()
        );
    }
}