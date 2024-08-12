namespace eCommerceRESTfulAPI.Application.DTOs
{
    public class CustomerCreateDto
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int Role { get; set; }
    }
}
