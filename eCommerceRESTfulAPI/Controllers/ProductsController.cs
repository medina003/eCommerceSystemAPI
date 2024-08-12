using eCommerceRESTfulAPI.Application.DTOs;
using eCommerceRESTfulAPI.Application.Interfaces.Services;
using eCommerceRESTfulAPI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductCreateDto productCreateDto)
    {
        var product = new Product
        {
            Name = productCreateDto.Name,
            Description = productCreateDto.Description,
            Price = productCreateDto.Price,
            StockQuantity = productCreateDto.StockQuantity,
            CategoryId = productCreateDto.CategoryId,
            Localizations = productCreateDto.Localizations.Select(loc => new ProductLocalization
            {
                LanguageCode = loc.LanguageCode,
                Name = loc.Name,
                Description = loc.Description
            }).ToList()
        };

        var createdProduct = await _productService.CreateAsync(product);
        return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(int id, [FromHeader] string languageCode)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        var localizedProduct = GetLocalizedProduct(product, languageCode);
        return Ok(localizedProduct);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto productUpdateDto)
    {
        if (id != productUpdateDto.Id)
        {
            return BadRequest("Product ID mismatch.");
        }

        var productToUpdate = new Product
        {
            Id = productUpdateDto.Id,
            Name = productUpdateDto.Name,
            Description = productUpdateDto.Description,
            Price = productUpdateDto.Price,
            StockQuantity = productUpdateDto.StockQuantity,
            CategoryId = productUpdateDto.CategoryId,
            Localizations = productUpdateDto.Localizations.Select(loc => new ProductLocalization
            {
                LanguageCode = loc.LanguageCode,
                Name = loc.Name,
                Description = loc.Description
            }).ToList()
        };

        await _productService.UpdateAsync(productToUpdate);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        await _productService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts(
     [FromHeader] string languageCode,
     [FromHeader] int page = 1,
     [FromHeader] int pageSize = 10)
    {
        var products = await _productService.GetAllAsync(page, pageSize);

        var localizedProducts = products.Select(p => GetLocalizedProduct(p, languageCode)).ToList();
        return Ok(localizedProducts);
    }


    private ProductDto GetLocalizedProduct(Product product, string languageCode)
    {
        var localization = product.Localizations.FirstOrDefault(loc => loc.LanguageCode == languageCode);

        return new ProductDto
        {
            Id = product.Id,
            Name = localization?.Name ?? product.Name,
            Description = localization?.Description ?? product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId
        };
    }
}
