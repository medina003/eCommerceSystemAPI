using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerceRESTfulAPI.Domain.Entities;

namespace eCommerceRESTfulAPI.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync(int page, int pageSize);
        Task<Category> GetByIdAsync(int id);
        Task<Category> CreateAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }
}
