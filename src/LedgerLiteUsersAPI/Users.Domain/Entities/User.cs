using Users.Domain.Exceptions.UserExceptions;
using Users.Domain.Validators;

namespace Users.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateOnly Birthdate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }

        // Relationships:
        public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

        public User(string name, string surname, DateOnly birthdate, string email, string phone, bool isActive)
        {
            if(!new UserDataValidator().IsValidBirthdate(birthdate)) throw new InvalidUserAgeException(birthdate);
            if(!new UserDataValidator().IsValidEmail(email)) throw new InvalidUserEmailException(email);
            if(!new UserDataValidator().IsValidPhone(phone)) throw new InvalidUserPhoneException(phone);

            Name = name;
            Surname = surname;
            Birthdate = birthdate;
            Email = email;
            Phone = phone;
            IsActive = isActive;
        }
    }
}