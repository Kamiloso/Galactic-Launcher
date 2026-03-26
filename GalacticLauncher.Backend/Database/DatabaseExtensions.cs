using GalacticLauncher.Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace GalacticLauncher.Backend.Database;

public static class DatabaseExtensions
{
    private static readonly Version Version = new(8, 0, 45);
    private static readonly MySqlServerVersion ServerVersion = new(Version);

    public static IServiceCollection AddGalacticDatabase(this IServiceCollection srv)
    {
        // TODO: ADD Valid live connection for the Database in the appsettings
        srv.AddDbContext<AppDbContext>((provider, options) =>
        {
            var dbConfig = provider.GetRequiredService<AppConfig.DatabaseSection>();
            string connString = MakeConnectionString(dbConfig);

            options.UseMySql(connString, ServerVersion)
                .UseSnakeCaseNamingConvention(); // snake_case!
        });
        
        srv.AddScoped<ILauncherRepository, LauncherRepository>(); // Instead of Singleton so that the database doesnt break
        // srv.AddHostedService<DatabaseMigrator>();
        return srv;
    }

    private static string MakeConnectionString(AppConfig.DatabaseSection config)
    {
        var builder = new DbConnectionStringBuilder
        {
            { "Server", config.Address },
            { "Port", config.Port },
            { "Database", config.Database },
            { "User ID", config.User },
            { "Password", config.Password }
        };

        return builder.ConnectionString;
    }
}