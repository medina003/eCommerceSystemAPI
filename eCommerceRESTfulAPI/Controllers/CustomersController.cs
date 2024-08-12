using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using eCommerceRESTfulAPI.Domain.Entities;
using eCommerceRESTfulAPI.Infrastructure.Data.DbContexts;
using System.Globalization;
using System.Threading.Tasks;

namespace eCommerceRESTfulAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly eCommerceSystemDbContext _context;
        private readonly IStringLocalizer<CustomersController> _localizer;

        public CustomersController(eCommerceSystemDbContext context, IStringLocalizer<CustomersController> localizer)
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
        public async Task<ActionResult<IEnumerable<Customer>>> RetrieveAllCustomers(
            [FromHeader(Name = "Accept-Language")] string language = null)
        {
            SetCulture(language);

            var customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }

        [HttpGet("by-id")]
        public async Task<ActionResult<Customer>> RetrieveCustomer(
            [FromHeader] int id,
            [FromHeader(Name = "Accept-Language")] string language = null)
        {
            SetCulture(language);

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound(_localizer["CustomerNotFound"]);
            return Ok(customer);
        }

     

        [HttpPut]
        public async Task<IActionResult> UpdateCustomer(
            [FromHeader] int id,
            [FromBody] Customer customer,
            [FromHeader(Name = "Accept-Language")] string language = null)
        {
            SetCulture(language);

            if (id != customer.Id) return BadRequest();
            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(
            [FromHeader] int id,
            [FromHeader(Name = "Accept-Language")] string language = null)
        {
            SetCulture(language);
            foreach (var resource in _localizer.GetAllStrings())
            {
                Console.WriteLine($"{resource.Name} = {resource.Value}");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound(_localizer["CustomerNotFound"]);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
