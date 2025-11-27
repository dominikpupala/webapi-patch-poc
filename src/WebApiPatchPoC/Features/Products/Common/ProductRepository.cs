using Dapper;
using WebApiPatchPoC.Data;
using WebApiPatchPoC.Features.Products.Domain;

namespace WebApiPatchPoC.Features.Products.Common;

internal sealed class ProductRepository(IDbConnectionFactory dbConnectionFactory) : IProductRepository
{
    public async Task<Product?> GetBySku(string sku)
    {
        using var connection = await dbConnectionFactory.Create();

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

        var product = await connection.QuerySingleOrDefaultAsync<Product>(sql, new
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

    public async Task<bool> Save(Product product)
    {
        using var connection = await dbConnectionFactory.Create();

        const string sql = """
            UPDATE dbo.Products
            SET
                Name = @Name,
                ImgUri = @ImgUri,
                Price = @Price,
                Description = @Description
            WHERE Sku = @Sku;

            SELECT @@ROWCOUNT;
            """;

        var rowsAffected = await connection.ExecuteScalarAsync<int>(sql, new
        {
            // Use DbString with IsAnsi when filtering on VARCHAR column to avoid implicit conversion.
            Sku = new DbString
            {
                Value = product.Sku,
                IsAnsi = true,
                Length = 50
            },
            product.Name,
            product.ImgUri,
            product.Price,
            product.Description
        });

        return rowsAffected > 0;
    }
}
