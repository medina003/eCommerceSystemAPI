using eCommerceRESTfulAPI.Domain.Entities;

namespace eCommerceRESTfulAPI.Application.Interfaces.Hashing
{
    public interface IPasswordHasherService
    {
        string HashPassword(User user, string password);
        bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword);
    }
}
