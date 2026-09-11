namespace Users.Domain.Interfaces.Validators
{
    public interface IBankAccountDataValidator
    {
        bool IsValidAccountNumber(string accountNumber);
        bool IsValidAgency(string agency);
    }
}