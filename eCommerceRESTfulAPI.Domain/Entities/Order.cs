using eCommerceRESTfulAPI.Domain.Entities;
using eCommerceRESTfulAPI.Domain.Enum;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public OrderStatus Status { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = null!;

    public decimal TotalPrice { get; private set; }

    public void CalculateTotalPrice()
    {
        TotalPrice = OrderItems.Sum(item => item.Quantity * item.Product.Price);
    }
}
