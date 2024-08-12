using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using eCommerceRESTfulAPI.Domain.Entities;
using eCommerceRESTfulAPI.Infrastructure.Data.DbContexts;
using eCommerceRESTfulAPI.Domain.Common;
using eCommerceRESTfulAPI.Application.DTOs; 
using System.Globalization;
using System.Threading.Tasks;

namespace eCommerceRESTfulAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly eCommerceSystemDbContext _context;
        private readonly IStringLocalizer<CategoriesController> _localizer;

        public CategoriesController(eCommerceSystemDbContext context, IStringLocalizer<CategoriesController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        private void SetCulture(string language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                CultureInfo.CurrentCulture = new CultureInfo(language);
                CultureInfo.CurrentUICulture = new CultureInfo(language);
            }
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<Category>>> RetrieveCategories(
            [FromHeader] int page = 1,
            [FromHeader] int pageSize = 10,
            [FromHeader(Name = "Accept-Language")] string language = null)
        {
            SetCulture(language);

            var totalCount = await _context.Categories.CountAsync();
            var items = await _context.Categories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new PagedResult<Category>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> RetrieveCategory(
            [FromHeader] int id,
            [FromHeader(Name = "Accept-Language")] string language = null)
        {
            SetCulture(language);

            var category = await _context.Categories.Include(c => c.Localizations).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound(_localizer["CategoryNotFound"]);
            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> CreateNewCategory([FromBody] CategoryCreateDto categoryCreateDto)
        {
            var category = new Category
            {
                Name = categoryCreateDto.Name,
                Description = categoryCreateDto.Description,
                Localizations = categoryCreateDto.Localizations.Select(loc => new CategoryLocalization
                {
                    LanguageCode = loc.LanguageCode,
                    Name = loc.Name
                }).ToList()
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(RetrieveCategory), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(
            [FromHeader] int id,
            [FromBody] Category category,
            [FromHeader(Name = "Accept-Language")] string language = null)
        {
            SetCulture(language);

            if (id != category.Id) return BadRequest();
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(
            [FromHeader] int id,
            [FromHeader(Name = "Accept-Language")] string language = null)
        {
            SetCulture(language);

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound(_localizer["CategoryNotFound"]);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
