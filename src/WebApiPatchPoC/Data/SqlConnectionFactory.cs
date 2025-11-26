using Microsoft.Data.SqlClient;
using System.Data;

namespace WebApiPatchPoC.Data;

#pragma warning disable CA1812 // Instantiated by DI container
internal sealed class SqlConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
#pragma warning restore CA1812
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

    public async Task<IDbConnection> Create()
    {
        var connection = new SqlConnection(_connectionString);
        return connection;
    }
}
