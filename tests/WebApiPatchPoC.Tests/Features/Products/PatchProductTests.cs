using NSubstitute;
using OneOf;
using OneOf.Types;
using System.Text.Json;
using WebApiPatchPoC;
using WebApiPatchPoC.Features.Products.Common;
using WebApiPatchPoC.Features.Products.Domain;
using WebApiPatchPoC.Features.Products.PatchProduct;

namespace WebApiPatchPoC.Tests.Features.Products;

public class PatchProductTests
{
    [Fact]
    public async Task Handler_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var mockRepository = Substitute.For<IProductRepository>();
        mockRepository
            .GetBySku("INVALID-SKU")
            .Returns((Product?)null);

        var handler = new PatchProductHandler(mockRepository);
        var command = new PatchProductHandler.Command(
            "NONEXISTENT",
            new Unknown(),
            new Unknown(),
            new Unknown(),
            new Unknown());

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.IsT1);
        Assert.IsType<NotFound>(result.AsT1);
    }

    [Fact]
    public async Task Handler_UpdatesDescription_WhenDescriptionIsProvided()
    {
        // Arrange
        var existingProduct = new Product(
            "SKU001",
            "Product A",
            "https://example.com/a.jpg",
            19.99m,
            "Old");

        var mockRepository = Substitute.For<IProductRepository>();
        mockRepository
            .GetBySku("SKU001")
            .Returns(existingProduct);

        mockRepository
            .Save(Arg.Any<Product>())
            .Returns(true);

        var handler = new PatchProductHandler(mockRepository);
        var command = new PatchProductHandler.Command(
            "SKU001",
            new Unknown(),
            new Unknown(),
            new Unknown(),
            OneOf<string, None, Unknown>.FromT0("New Description"));

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.IsT0);
        await mockRepository
            .Received(1)
            .Save(Arg.Is<Product>(p => p.Description == "New Description"));
    }

    [Fact]
    public async Task Handler_ClearsDescription_WhenDescriptionIsNone()
    {
        // Arrange
        var existingProduct = new Product(
            "SKU001",
            "Product A",
            "https://example.com/a.jpg",
            19.99m,
            "Old");

        var mockRepository = Substitute.For<IProductRepository>();

        mockRepository
            .GetBySku("SKU001")
            .Returns(existingProduct);

        mockRepository
            .Save(Arg.Any<Product>())
            .Returns(true);

        var handler = new PatchProductHandler(mockRepository);
        var command = new PatchProductHandler.Command(
            "SKU001",
            new Unknown(),
            new Unknown(),
            new Unknown(),
            new None());

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.IsT0);
        await mockRepository.Received(1).Save(Arg.Is<Product>(p => p.Description == null));
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenSaveFails()
    {
        // Arrange
        var existingProduct = new Product(
            "SKU001",
            "Product A",
            "https://example.com/a.jpg",
            19.99m,
            "Desc");

        var mockRepository = Substitute.For<IProductRepository>();

        mockRepository
            .GetBySku("SKU001")
            .Returns(existingProduct);

        mockRepository
            .Save(Arg.Any<Product>())
            .Returns(false);

        var handler = new PatchProductHandler(mockRepository);
        var command = new PatchProductHandler.Command(
            "SKU001",
            new Unknown(),
            new Unknown(),
            new Unknown(),
            new Unknown());

        // Act
        var result = await handler.Handle(command);

        // Assert
        Assert.True(result.IsT1);
        Assert.IsType<NotFound>(result.AsT1);
    }

    private readonly PatchProductEndpoint.PatchProductValidator _validator = new();

    [Fact]
    public void Validate_Success_WhenSkuIsValid()
    {
        // Arrange
        var request = new PatchProductEndpoint.Request("VALID-SKU-123", JsonDocument.Parse("{}").RootElement);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Fails_WhenSkuIsEmpty()
    {
        // Arrange
        var request = new PatchProductEndpoint.Request("", JsonDocument.Parse("{}").RootElement);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "SKU is required");
    }

    [Fact]
    public void Validate_Fails_WhenSkuExceedsMaxLength()
    {
        // Arrange
        var request = new PatchProductEndpoint.Request(new string('A', 51), JsonDocument.Parse("{}").RootElement);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "SKU cannot exceed 50 characters");
    }

    [Fact]
    public void Validate_Fails_WhenSkuHasLeadingWhitespace()
    {
        // Arrange
        var request = new PatchProductEndpoint.Request(" SKU123", JsonDocument.Parse("{}").RootElement);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "SKU cannot have leading or trailing whitespace");
    }
}
