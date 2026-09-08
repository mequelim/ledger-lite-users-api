namespace LedgeLiteUsers.Domain.Interfaces.Validators
{
    public interface IUserDataValidator
    {
        bool IsValidEmail(string email);
        bool IsValidPhone(string phone);
    }
}