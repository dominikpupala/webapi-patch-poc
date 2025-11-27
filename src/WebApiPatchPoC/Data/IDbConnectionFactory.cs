using System.Data;

namespace WebApiPatchPoC.Data;

internal interface IDbConnectionFactory
{
    Task<IDbConnection> Create();
}
