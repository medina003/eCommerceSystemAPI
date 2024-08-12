using eCommerceRESTfulAPI.Application.Interfaces.Hashing;
using eCommerceRESTfulAPI.Domain.Entities;
using System;
using System.Security.Cryptography;
using System.Text;

namespace eCommerceRESTfulAPI.Application.Hashing
{
    public class PasswordHasherService : IPasswordHasherService
    {
        public string HashPassword(User user, string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hashedBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            var hashedInput = HashPassword(user, providedPassword);
            return hashedInput == hashedPassword;
        }
    }
}
