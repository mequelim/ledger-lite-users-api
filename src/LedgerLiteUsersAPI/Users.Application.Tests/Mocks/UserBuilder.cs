using Users.Domain.Entities;

namespace Users.Application.Tests.Mocks
{
    public class UserBuilder
    {
        private const string Name = "Pedro";
        private const string Surname = "Mequelim";
        private readonly DateOnly _birthdate = new(2002, 02, 15);
        private const string Email = "pedro@email.com";
        private const string Phone = "+55 (41) 9 1234-4567";
        private const bool IsActive = true;

        // Relationships:
        private BankAccount? _bankAccount;

        // Methods:
        public UserBuilder WithBankAccount(BankAccount bankAccount)
        {
            _bankAccount = bankAccount;
            return this;
        }

        public User Build()
        {
            User user = new(
                Name, Surname, _birthdate,
                Email, Phone, IsActive
            );

            user.BankAccount = _bankAccount ?? BankAccountFactory.CreateDefault(user.Id);

            return user;
        }
    }
}