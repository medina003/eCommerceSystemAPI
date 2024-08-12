using eCommerceRESTfulAPI.Domain.Entities;
using System.Threading.Tasks;

namespace eCommerceRESTfulAPI.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<Order> CreateAsync(Order order);
        Task<Order> GetByIdAsync(int id);
        Task<Order> UpdateAsync(Order order);
        Task DeleteAsync(int id);
        Task RestockAsync(int productId, int quantity);
        Task<Product> GetProductByIdAsync(int productId);
        Task DecreaseStockAsync(int productId, int quantity); 
    }
}
