using Dapper;
using WebApiPatchPoC;
using WebApiPatchPoC.Data;

namespace WebApiPatchPoC.Features.Products.Common;

internal sealed class ProductReadService(IDbConnectionFactory connectionFactory) : IProductReadService
{
    public async Task<IReadOnlyList<ProductReadModel>> GetProducts()
    {
        using var connection = await connectionFactory.Create();

        const string sql = """
            SELECT
                Sku,
                Name,
                ImgUri,
                Price,
                Description
            FROM dbo.Products
            ORDER BY Name;
            """;

        var products = await connection.QueryAsync<ProductReadModel>(sql);

        return [.. products];
    }

    public async Task<ProductReadModel?> GetProductBySku(string sku)
    {
        using var connection = await connectionFactory.Create();

        const string sql = """
            SELECT
                Sku,
                Name,
                ImgUri,
                Price,
                Description
            FROM dbo.Products
            WHERE Sku = @Sku;
            """;

        var product = await connection.QuerySingleOrDefaultAsync<ProductReadModel>(sql, new
        {
            // Use DbString with IsAnsi when filtering on VARCHAR column to avoid implicit conversion.
            Sku = new DbString
            {
                Value = sku,
                IsAnsi = true,
                Length = 50
            }
        });

        return product;
    }
}
