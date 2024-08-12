namespace eCommerceRESTfulAPI.Application.DTOs
{
    public class ProductLocalizationDto
    {
        public string LanguageCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}