using System.Text.RegularExpressions;
using LedgeLiteUsers.Domain.Interfaces.Validators;

namespace LedgeLiteUsers.Domain.Validators
{
    public class UserDataValidator : IUserDataValidator
    {
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new(@"^(?:\+?\s*55\s*)?(?:\(?\d{2}\)?\s*)?(?:9\s*)?\d{4}-?\d{4}$", RegexOptions.Compiled);

        // Methods:
        public bool IsValidEmail(string email) => (!string.IsNullOrWhiteSpace(email) && (EmailRegex.IsMatch(email)));

        public bool IsValidPhone(string phone) => (!string.IsNullOrWhiteSpace(phone) && (PhoneRegex.IsMatch(phone)));

        public bool IsValidBirthdate(DateOnly birthdate)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            int userAge = today.Year - birthdate.Year;

            if(birthdate < today.AddYears(-userAge)) userAge--;

            return (userAge is >= 18 and <= 100);
        }
    }
}