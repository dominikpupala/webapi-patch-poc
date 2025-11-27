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
}
