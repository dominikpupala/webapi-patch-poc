using NSubstitute;
using OneOf.Types;
using WebApiPatchPoC.Features.Products.Common;
using WebApiPatchPoC.Features.Products.GetProductBySku;

namespace WebApiPatchPoC.Tests.Features.Products;

public class GetProductBySkuTests
{
    [Fact]
    public async Task Handler_CallsServiceWithCorrectSku()
    {
        // Arrange
        var mockService = Substitute.For<IProductReadService>();
        mockService
            .GetProductBySku(Arg.Any<string>())
            .Returns((ProductReadModel?)null);

        var handler = new GetProductBySkuHandler(mockService);
        var query = new GetProductBySkuHandler.Query("TEST-SKU-123");

        // Act
        await handler.Handle(query);

        // Assert
        await mockService.Received(1).GetProductBySku("TEST-SKU-123");
    }

    [Fact]
    public async Task Handler_ReturnsProduct_WhenProductExists()
    {
        // Arrange
        var mockService = Substitute.For<IProductReadService>();
        var expectedProduct = new ProductReadModel(
            "SKU001",
            "Product A",
            "https://example.com/a.jpg",
            19.99m,
            "Description A");

        mockService
            .GetProductBySku("SKU001")
            .Returns(expectedProduct);

        var handler = new GetProductBySkuHandler(mockService);
        var query = new GetProductBySkuHandler.Query("SKU001");

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.True(result.IsT0);
        var product = result.AsT0;
        Assert.Equal("SKU001", product.Sku);
        Assert.Equal("Product A", product.Name);
        Assert.Equal("https://example.com/a.jpg", product.ImgUri);
        Assert.Equal(19.99m, product.Price);
        Assert.Equal("Description A", product.Description);
    }

    [Fact]
    public async Task Handler_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var mockService = Substitute.For<IProductReadService>();
        mockService
            .GetProductBySku("INVALID-SKU")
            .Returns((ProductReadModel?)null);

        var handler = new GetProductBySkuHandler(mockService);
        var query = new GetProductBySkuHandler.Query("INVALID-SKU");

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.True(result.IsT1);
        Assert.IsType<NotFound>(result.AsT1);
    }
}
