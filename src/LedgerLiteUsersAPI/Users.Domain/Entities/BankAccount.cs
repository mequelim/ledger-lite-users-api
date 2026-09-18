using Users.Domain.Entities.Enums;
using Users.Domain.Exceptions.BankAccount;
using Users.Domain.Validators;

namespace Users.Domain.Entities
{
    public class BankAccount : BaseEntity
    {
        public string BankName { get; set; }
        public string? Holder { get; set; }
        public string AccountNumber { get; set; }
        public string Agency { get; set; }
        public BankAccountType BankAccountType { get; init; }

        // Foreign Keys (FKs):
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public BankAccount(
            string bankName,
            string? holder,
            string accountNumber,
            string agency,
            BankAccountType bankAccountType,
            Guid userId
        )
        {
            if(!new BankAccountDataValidator().IsValidAccountNumber(accountNumber)) throw new InvalidAccountNumberException(accountNumber);
            if(!new BankAccountDataValidator().IsValidAgency(agency)) throw new InvalidAgencyException(agency);

            BankName = bankName;
            Holder = holder;
            AccountNumber = accountNumber;
            Agency = agency;
            BankAccountType = bankAccountType;
            UserId = userId;
        }
    }
}