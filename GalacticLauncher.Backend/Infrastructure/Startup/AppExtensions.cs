using GalacticLauncher.Core;

namespace GalacticLauncher.Backend.Infrastructure.Startup;

public static class AppExtensions
{
    public static void ConfigureMiddleware(this WebApplication app, AppConfig config)
    {
        if (config.Listener.UseForwardedFor)
        {
            app.UseForwardedHeaders();
        }

        if (Utils.IsDevelopment)
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

        if (Utils.IsDevelopment)
        {
            logger.LogInformation(
                "Swagger initialized. Open: {Address}/swagger/index.html", Utils.Address);

            logger.LogWarning(
                "Running in DEBUG / DEVELOPMENT mode.");
        }
    }
}