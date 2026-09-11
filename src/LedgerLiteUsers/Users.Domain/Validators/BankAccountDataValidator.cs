using System.Text.RegularExpressions;
using Users.Domain.Interfaces.Validators;

namespace Users.Domain.Validators
{
    public class BankAccountDataValidator : IBankAccountDataValidator
    {
        private static readonly Regex AgencyRegex = new(@"^\d{4}(?:-[\dXdx])?$", RegexOptions.Compiled);
        private static readonly Regex AccountNumberRegex = new(@"^\d{5,12}-[\dXdx]$", RegexOptions.Compiled);

        // Methods:
        public bool IsValidAccountNumber(string accountNumber) => (!string.IsNullOrWhiteSpace(accountNumber) && (AccountNumberRegex.IsMatch(accountNumber)));

        public bool IsValidAgency(string agency) => (!string.IsNullOrWhiteSpace(agency) && (AgencyRegex.IsMatch(agency)));
    }
}