using GalacticLauncher.Backend.Database;
using GalacticLauncher.Backend.Repositories;
using GalacticLauncher.Core;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Threading.RateLimiting;
using GalacticLauncher.Core.Interfaces;
using static GalacticLauncher.Backend.AppConfig.RateLimitingSection;

namespace GalacticLauncher.Backend;

public static class ServiceExtensions
{
    public static AppConfig ConfigureAppConfig(this IServiceCollection services, IConfiguration configuration)
    {
        IConfiguration section = configuration.GetSection("AppConfig");

        AppConfig? config = section.Get<AppConfig>();
        if (Utils.IsNullRecursive(config))
        {
            throw new InvalidOperationException("AppConfig section is missing or invalid.");
        }

        services.AddSingleton(config!);
        return config!;
    }

    public static void ConfigureForwardedFor(this IServiceCollection services, AppConfig config)
    {
        if (config.Listener.UseForwardedFor)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }
    }

    public static void ConfigureRateLimiters(this IServiceCollection services, AppConfig config)
    {
        static RateLimitPartition<string> RateLimitPartition(
            HttpContext context, AppConfig config, RateLimitRule policy)
        {
            IPAddress? ip = context.Connection.RemoteIpAddress;

            if (ip?.IsIPv4MappedToIPv6 == true)
            {
                ip = ip.MapToIPv4();
            }

            AddressFamily family = ip?.AddressFamily ?? AddressFamily.Unknown;

            int lenV4 = Math.Clamp(config.Listener.PrefixIPv4, 0, 32) / 8;
            int lenV6 = Math.Clamp(config.Listener.PrefixIPv6, 0, 128) / 8;

            string uniqueNetworkString = family switch
            {
                AddressFamily.InterNetwork when ip is not null =>
                    Convert.ToHexString(ip.GetAddressBytes().AsSpan(0, lenV4)),

                AddressFamily.InterNetworkV6 when ip is not null =>
                    Convert.ToHexString(ip.GetAddressBytes().AsSpan(0, lenV6)),

                _ => "???"
            };

            return System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: uniqueNetworkString,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    Window = TimeSpan.FromSeconds(policy.Seconds),
                    PermitLimit = policy.Limit,
                    QueueLimit = 0
                });
        }

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy("LowCost", context => RateLimitPartition(context, config, config.Limiter.LowCost));
            options.AddPolicy("MediumCost", context => RateLimitPartition(context, config, config.Limiter.MediumCost));
            options.AddPolicy("HighCost", context => RateLimitPartition(context, config, config.Limiter.HighCost));
        });
    }

    public static IServiceCollection AddGalacticDatabase(this IServiceCollection srv, AppConfig config)
    {
        var rawVersion = new Version(8, 0, 45);
        var sqlVersion = new MySqlServerVersion(rawVersion);

        // TODO: ADD Valid live connection for the Database in the appsettings
        srv.AddDbContext<AppDbContext>((provider, options) =>
        {
            var builder = new DbConnectionStringBuilder
            {
                { "Server", config.Database.Address },
                { "Port", config.Database.Port },
                { "Database", config.Database.Database },
                { "User ID", config.Database.User },
                { "Password", config.Database.Password }
            };

            options.UseMySql(builder.ConnectionString, sqlVersion)
                .UseSnakeCaseNamingConvention(); // snake_case!
        });

        srv.AddScoped<ILauncherRepository, LauncherRepository>(); // Instead of Singleton so that the database doesnt break
        // srv.AddHostedService<DatabaseMigrator>();
        return srv;
    }
}