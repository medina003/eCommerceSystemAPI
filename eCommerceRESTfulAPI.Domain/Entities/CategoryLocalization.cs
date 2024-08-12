namespace eCommerceRESTfulAPI.Domain.Entities
{
    public class CategoryLocalization
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string? LanguageCode { get; set; } 
        public string? Name { get; set; }
    }
}