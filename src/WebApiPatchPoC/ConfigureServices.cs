using WebApiPatchPoC.Data;

namespace WebApiPatchPoC;

internal static class ConfigureServices
{
    extension(WebApplicationBuilder builder)
    {
        public void AddServices()
        {
            builder.Services.AddOpenApi();

            builder.AddDatabase();
        }

        private void AddDatabase()
        {
            builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        }
    }
}
