namespace Users.Domain.Interfaces.Validators
{
    public interface IUserDataValidator
    {
        bool IsValidEmail(string email);
        bool IsValidPhone(string phone);
        bool IsValidBirthdate(DateOnly birthdate);
    }
}