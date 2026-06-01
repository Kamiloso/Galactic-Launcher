using GalacticLauncher.Core;
using Microsoft.AspNetCore.HttpOverrides;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Threading.RateLimiting;

namespace GalacticLauncher.Backend.Infrastructure.Startup;

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

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        if (Utils.IsDevelopment)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SupportNonNullableReferenceTypes();
                c.EnableAnnotations();
            });
        }
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
        static RateLimitPartition<string> GetRateLimitPartition(
            HttpContext context, AppConfig config,
            AppConfig.RateLimitingSection.RateLimitRule policy)
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

                _ => "Unknown"
            };

            return RateLimitPartition.GetFixedWindowLimiter(
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

            options.AddPolicy("LowCost", context => GetRateLimitPartition(context, config, config.Limiter.LowCost));
            options.AddPolicy("MediumCost", context => GetRateLimitPartition(context, config, config.Limiter.MediumCost));
            options.AddPolicy("HighCost", context => GetRateLimitPartition(context, config, config.Limiter.HighCost));
            options.AddPolicy("TelemetryCost", context => GetRateLimitPartition(context, config, config.Limiter.TelemetryCost));
            options.AddPolicy("ReqCost", context => GetRateLimitPartition(context, config, config.Limiter.ReqCost));
        });
    }

    public static void AddDatabase(this IServiceCollection srv, AppConfig config)
    {
        var builder = new DbConnectionStringBuilder
        {
            { "Server", config.Database.Address },
            { "Port", config.Database.Port },
            { "Database", config.Database.Database },
            { "User ID", config.Database.User },
            { "Password", config.Database.Password }
        };

        string connectionString = builder.ConnectionString;

        srv.AddScoped(_ => new DbSession(connectionString));
    }
}
