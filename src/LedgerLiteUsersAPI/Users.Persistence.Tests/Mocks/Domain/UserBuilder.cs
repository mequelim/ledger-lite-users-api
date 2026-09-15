using Users.Domain.Entities;

namespace Users.Persistence.Tests.Mocks.Domain
{
    public class UserBuilder
    {
        private const string Name = "Pedro";
        private const string Surname = "Mequelim";
        private readonly DateOnly _dateOfBirth = new(2002, 02, 15);
        private const string Email = "pedro@email.com";
        private const string Phone = "+55 (41) 9 1234-4567";
        private const bool IsActive = true;

        // Relationships:
        private readonly List<BankAccount> _bankAccounts = new();

        // Methods:
        public UserBuilder WithBankAccount(BankAccount bankAccount)
        {
            _bankAccounts.Add(bankAccount);
            return this;
        }

        public User Build()
        {
            User user = new(
                Name, Surname, _dateOfBirth,
                Email, Phone, IsActive
            );

            if(_bankAccounts.Count == 0) _bankAccounts.Add(BankAccountFactory.CreateDefault(user.Id));

            foreach(BankAccount bankAccount in _bankAccounts) user.BankAccounts.Add(bankAccount);

            return user;
        }
    }
}