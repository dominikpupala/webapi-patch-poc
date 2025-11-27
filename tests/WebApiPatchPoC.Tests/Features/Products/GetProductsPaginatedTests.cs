using FluentValidation.TestHelper;
using NSubstitute;
using WebApiPatchPoC.Common;
using WebApiPatchPoC.Features.Products.Common;
using WebApiPatchPoC.Features.Products.GetProductsPaginated;

namespace WebApiPatchPoC.Tests.Features.Products;

public class GetProductsPaginatedTests
{
    [Fact]
    public async Task Handler_ReturnsPaginatedProducts_WhenServiceSucceeds()
    {
        // Arrange
        var mockService = Substitute.For<IProductReadService>();
        var expectedProducts = new List<ProductReadModel>
        {
            new("SKU001", "Product A", "https://example.com/img1.jpg", 10.99m, "Description for product A"),
            new("SKU002", "Product B", "https://example.com/img2.jpg", 15.50m, "Description for product B"),
            new("SKU003", "Product C", "https://example.com/img3.jpg", 20.00m, "Description for product C"),
            new("SKU004", "Product D", "https://example.com/img4.jpg", 25.25m, "Description for product D"),
            new("SKU005", "Product E", "https://example.com/img5.jpg", 30.99m, "Description for product E"),
            new("SKU006", "Product F", "https://example.com/img6.jpg", 12.75m, "Description for product F"),
            new("SKU007", "Product G", "https://example.com/img7.jpg", 18.49m, "Description for product G"),
            new("SKU008", "Product H", "https://example.com/img8.jpg", 22.99m, "Description for product H"),
            new("SKU009", "Product I", "https://example.com/img9.jpg", 27.50m, "Description for product I"),
            new("SKU010", "Product J", "https://example.com/img10.jpg", 33.33m, "Description for product J")
        }
        .AsReadOnly();

        var paginatedResult = new PaginatedResult<ProductReadModel>(
            expectedProducts,
            TotalCount: 10,
            PageNumber: 1,
            PageSize: 10);

        mockService
            .GetProductsPaginated(1, 10)
            .Returns(paginatedResult);

        var handler = new GetProductsPaginatedHandler(mockService);
        var query = new GetProductsPaginatedHandler.Query(1, 10);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal("SKU001", result.Items[0].Sku);
        Assert.Equal("Product B", result.Items[1].Name);
    }

    [Fact]
    public async Task Handler_ReturnsEmptyPage_WhenPageExceedsTotalPages()
    {
        // Arrange
        var mockService = Substitute.For<IProductReadService>();
        var paginatedResult = new PaginatedResult<ProductReadModel>(
            [],
            TotalCount: 0,
            PageNumber: 10,
            PageSize: 10);

        mockService
            .GetProductsPaginated(10, 10)
            .Returns(paginatedResult);

        var handler = new GetProductsPaginatedHandler(mockService);
        var query = new GetProductsPaginatedHandler.Query(10, 10);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(5, 25)]
    [InlineData(100, 100)]
    public async Task Handler_HandlesVariousPageSizes_Successfully(int pageNumber, int pageSize)
    {
        // Arrange
        var mockService = Substitute.For<IProductReadService>();
        var paginatedResult = new PaginatedResult<ProductReadModel>(
            [],
            TotalCount: 1000,
            PageNumber: pageNumber,
            PageSize: pageSize);

        mockService
            .GetProductsPaginated(pageNumber, pageSize)
            .Returns(paginatedResult);

        var handler = new GetProductsPaginatedHandler(mockService);
        var query = new GetProductsPaginatedHandler.Query(pageNumber, pageSize);

        // Act
        var result = await handler.Handle(query);

        // Assert
        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
    }

    private readonly GetProductsPaginatedEndpoint.RequestValidator _validator = new();

    [Theory]
    [InlineData(1, 10)]
    [InlineData(1, 1)]
    [InlineData(100, 100)]
    [InlineData(5, 50)]
    public void Validator_ShouldNotHaveError_WhenRequestIsValid(int pageNumber, int pageSize)
    {
        // Arrange
        var request = new GetProductsPaginatedEndpoint.Request(pageNumber, pageSize);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(-100, 10)]
    public void Validator_ShouldHaveError_WhenPageNumberIsInvalid(int pageNumber, int pageSize)
    {
        // Arrange
        var request = new GetProductsPaginatedEndpoint.Request(pageNumber, pageSize);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result
            .ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("Page number must be greater than 0");
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    [InlineData(1, 101)]
    [InlineData(1, 1000)]
    public void Validator_ShouldHaveError_WhenPageSizeIsInvalid(int pageNumber, int pageSize)
    {
        // Arrange
        var request = new GetProductsPaginatedEndpoint.Request(pageNumber, pageSize);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result
            .ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("Page size must be between 1 and 100");
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-1, -1)]
    [InlineData(0, 101)]
    public void Validator_ShouldHaveMultipleErrors_WhenMultipleFieldsAreInvalid(int pageNumber, int pageSize)
    {
        // Arrange
        var request = new GetProductsPaginatedEndpoint.Request(pageNumber, pageSize);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
