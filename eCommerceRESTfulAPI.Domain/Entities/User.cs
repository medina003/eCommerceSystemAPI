using eCommerceRESTfulAPI.Domain.Enums;

namespace eCommerceRESTfulAPI.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserRole Role { get; set; }
    }
}
