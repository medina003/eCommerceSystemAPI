using eCommerceRESTfulAPI.Domain.Entities;
using eCommerceRESTfulAPI.Domain.Interfaces.Repositories;
using eCommerceRESTfulAPI.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceRESTfulAPI.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly eCommerceSystemDbContext _dbContext;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(eCommerceSystemDbContext dbContext, IMemoryCache cache, ILogger<ProductRepository> logger)
        {
            _dbContext = dbContext;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(int page, int pageSize)
        {
            string cacheKey = $"Products_Page_{page}_Size_{pageSize}";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Product> cachedProducts))
            {
                _logger.LogInformation("Cache miss for key: {CacheKey}", cacheKey);

                cachedProducts = await _dbContext.Products
                    .Include(p => p.Localizations)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(cacheKey, cachedProducts, cacheEntryOptions);
            }
            else
            {
                _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            }

            return cachedProducts;
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            return await _dbContext.Products
                .Include(p => p.Localizations)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            if (await _dbContext.Products.AnyAsync(p => p.Name == product.Name))
            {
                throw new InvalidOperationException("A product with the same name already exists.");
            }

            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            _cache.Remove("Products");

            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            if (await _dbContext.Products.AnyAsync(p => p.Name == product.Name && p.Id != product.Id))
            {
                throw new InvalidOperationException("A product with the same name already exists.");
            }

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();

            _cache.Remove("Products");

            return product;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();

                _cache.Remove("Products");
            }
        }

        public async Task<IEnumerable<Product>> SearchAndFilterAsync(string query, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            string cacheKey = $"Search_Products_Query_{query}_CategoryId_{categoryId}_MinPrice_{minPrice}_MaxPrice_{maxPrice}";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Product> cachedProducts))
            {
                IQueryable<Product> queryable = _dbContext.Products.Include(p => p.Localizations);

                if (!string.IsNullOrWhiteSpace(query))
                {
                    queryable = queryable.Where(p => p.Name.Contains(query) || p.Description.Contains(query));
                }

                if (categoryId.HasValue)
                {
                    queryable = queryable.Where(p => p.CategoryId == categoryId.Value);
                }

                if (minPrice.HasValue)
                {
                    queryable = queryable.Where(p => p.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    queryable = queryable.Where(p => p.Price <= maxPrice.Value);
                }

                cachedProducts = await queryable.ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10)); 

                _cache.Set(cacheKey, cachedProducts, cacheEntryOptions);
            }

            return cachedProducts;
        }
    }
}
