namespace Users.Application.DTOs
{
    public class UserSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }

        // Nested relationship:
        public List<BankAccountDto> BankAccounts { get; set; } = [];
    }
}