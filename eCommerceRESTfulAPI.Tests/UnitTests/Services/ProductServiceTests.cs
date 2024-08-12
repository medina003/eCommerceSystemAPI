using Moq;
using Xunit;
using eCommerceRESTfulAPI.Application.Interfaces.Services;
using eCommerceRESTfulAPI.Domain.Interfaces.Repositories;
using eCommerceRESTfulAPI.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerceRESTfulAPI.Application.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _productService = new ProductService(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsProducts()
    {
        var mockProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1" },
            new Product { Id = 2, Name = "Product 2" }
        };

        _productRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(mockProducts);

        var result = await _productService.GetAllAsync(1, 10);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoProductsExist()
    {
        _productRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<Product>());

        var result = await _productService.GetAllAsync(1, 10);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ThrowsException_WhenRepositoryFails()
    {
        _productRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new System.Exception("Database error"));

        await Assert.ThrowsAsync<System.Exception>(() => _productService.GetAllAsync(1, 10));
    }
}
