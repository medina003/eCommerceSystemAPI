using System.Text.RegularExpressions;

namespace eCommerceRESTfulAPI.Application.Validators
{
    public class EmailValidator
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public bool IsValid(string email)
        {
            return EmailRegex.IsMatch(email);
        }
    }
}
