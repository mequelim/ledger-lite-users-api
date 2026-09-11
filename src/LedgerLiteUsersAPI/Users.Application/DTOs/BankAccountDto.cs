using Users.Domain.Entities.Enums;

namespace Users.Application.DTOs
{
    public class BankAccountDto
    {
        public Guid Id { get; set; }
        public string BankName { get; set; }
        public string? Holder { get; set; }
        public string AccountNumber { get; set; }
        public string Agency { get; set; }
        public BankAccountType BankAccountType { get; set; }
    }
}