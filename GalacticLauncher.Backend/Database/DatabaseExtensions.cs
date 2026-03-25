using GalacticLauncher.Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GalacticLauncher.Backend.Database;

public static class DatabaseExtensions
{
    public static IServiceCollection AddGalacticDatabase(this IServiceCollection srv)
    {
        // TODO: ADD Valid live connection for the Database in the appsettings
        srv.AddDbContext<AppDbContext>((provider, options) =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            
#if DEBUG
            var connection = config.GetConnectionString("LocalConnection") ?? throw new InvalidOperationException("LocalConnection string is missing!");
#else
            var connection = config.GetConnectionString("LiveConnection") ?? throw new InvalidOperationException("LiveConnection string is missing!");
#endif
            // Version can be changed depending on which one gets chosen (e.g., MySQL 8.0.45)
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 45));

            options.UseMySql(connection, serverVersion)
                .UseSnakeCaseNamingConvention(); // snake_case!
        });
        
        srv.AddScoped<ILauncherRepository, LauncherRepository>(); // Instead of Singleton so that the database doesnt break
        // srv.AddHostedService<DatabaseMigrator>();
        return srv;
    }
}