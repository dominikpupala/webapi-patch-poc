using NSubstitute;
using WebApiPatchPoC.Features.Products.Common;
using WebApiPatchPoC.Features.Products.GetProducts;

namespace WebApiPatchPoC.Tests.Features.Products;

public class GetProductsTests
{
    [Fact]
    public async Task Handler_ReturnsProductsFromService_WhenServiceSucceeds()
    {
        // Arrange
        var mockService = Substitute.For<IProductReadService>();
        var expectedProducts = new List<ProductReadModel>
        {
            new("SKU001", "Product A", "https://example.com/a.jpg", 19.99m, "Description A"),
            new("SKU002", "Product B", "https://example.com/b.jpg", 29.99m, "Description B")
        }
        .AsReadOnly();

        mockService
            .GetProducts()
            .Returns([.. expectedProducts]);

        var handler = new GetProductsHandler(mockService);

        // Act
        var result = await handler.Handle();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("SKU001", result[0].Sku);
        Assert.Equal("Product B", result[1].Name);
    }

    [Fact]
    public async Task Handler_ReturnsEmptyList_WhenServiceReturnsEmpty()
    {
        // Arrange
        var mockService = Substitute.For<IProductReadService>();
        mockService
            .GetProducts()
            .Returns([]);

        var handler = new GetProductsHandler(mockService);

        // Act
        var result = await handler.Handle();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
