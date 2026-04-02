using GalacticLauncher.Backend.Database;
using GalacticLauncher.Core;
using Microsoft.EntityFrameworkCore;

namespace GalacticLauncher.Backend;

public static class AppExtensions
{
    public static bool ConnectToDatabase(this WebApplication app, ILogger logger)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (Utils.HasArgumentCLI("--migrate"))
        {
            logger.LogInformation("Migrating database...");
            dbContext.Database.Migrate();
            logger.LogInformation("Database migrated successfully.");
            return false;
        }

        if (dbContext.Database.CanConnect())
        {
            logger.LogInformation("Connection with the database established.");
            return true;
        }
        else
        {
            logger.LogCritical("Connection with the database could not be established.");
            return false;
        }
    }

    public static void ConfigureMiddleware(this WebApplication app, AppConfig config)
    {
        if (config.Listener.UseForwardedFor)
        {
            app.UseForwardedHeaders();
        }

        if (Utils.IsDebug)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRateLimiter();
    }

    public static void LogStartup<T>(this WebApplication app, AppConfig config)
    {
        var logger = app.Services.GetRequiredService<ILogger<T>>();
        var lsConfig = config.Listener;

        logger.LogInformation("""
            ListenerConfig = {ListenerConfig}
            """, lsConfig);

        if (lsConfig.PrefixIPv4 % 8 != 0 ||
            lsConfig.PrefixIPv6 % 8 != 0)
        {
            logger.LogWarning("Network prefixes should be divisible by 8 to work properly.");
        }

        logger.LogInformation("Backend is running on: {Address}", Utils.Address);

        if (Utils.IsDebug)
        {
            logger.LogInformation(
                "Swagger initialized. Open: {Address}/swagger/index.html", Utils.Address);

            logger.LogWarning(
                "Running in DEBUG / DEVELOPMENT mode.");
        }
    }
}