namespace eCommerceRESTfulAPI.Domain.Entities
{
    public class ProductLocalization
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? LanguageCode { get; set; } 
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}