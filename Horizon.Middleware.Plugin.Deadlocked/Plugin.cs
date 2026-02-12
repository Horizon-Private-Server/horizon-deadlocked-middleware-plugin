using Horizon.Database.Plugins;
using Horizon.Middleware.Plugin.Deadlocked.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Horizon.Middleware.Plugin.Deadlocked
{
    public class Plugin : IPlugin
    {
        public void Register(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<CustomDbContext>((serviceProvider, dbContextBuilder) =>
            {
                var connectionStringPlaceHolder = configuration.GetConnectionString("DbConnection");
                string serverName = Environment.GetEnvironmentVariable("HORIZON_DB_SERVER") ?? throw new InvalidOperationException();
                string dbName = Environment.GetEnvironmentVariable("HORIZON_DB_NAME") ?? throw new InvalidOperationException();
                string dbUserName = Environment.GetEnvironmentVariable("HORIZON_DB_USER") ?? throw new InvalidOperationException();
                string dbPassword = Environment.GetEnvironmentVariable("HORIZON_MSSQL_SA_PASSWORD") ?? throw new InvalidOperationException();

                var connectionString = connectionStringPlaceHolder.Replace("{_SERVER}", serverName).Replace("{_DBNAME}", dbName).Replace("{_USERNAME}", dbUserName).Replace("{_PASSWORD}", dbPassword);
                dbContextBuilder.UseSqlServer(connectionString);
            });

            services.AddControllers()
                .AddApplicationPart(this.GetType().Assembly)
                .AddControllersAsServices(); // Optional: helps if plugins use DI
        }
    }
}
