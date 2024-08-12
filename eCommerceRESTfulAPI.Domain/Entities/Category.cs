namespace eCommerceRESTfulAPI.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ICollection<Product>? Products { get; set; }
        public List<CategoryLocalization>? Localizations { get; set; }

    }
}
