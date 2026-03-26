using GalacticLauncher.Core;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using System.Net.Sockets;
using System.Threading.RateLimiting;
using RateLimitRule = GalacticLauncher.Backend.AppConfig.RateLimitingSection.RateLimitRule;

namespace GalacticLauncher.Backend.Socket;

internal sealed class RecvBuilder
{
    private readonly List<Action<IServiceCollection>> _serviceConfigs = [];

    private AppConfig? _appConfig = null;

    public AppConfig FullConfig => _appConfig ?? throw NoConfig();
    public AppConfig.DatabaseSection DatabaseConfig => _appConfig?.DatabaseConfig ?? throw NoConfig();
    public AppConfig.ListenerSection ListenerConfig => _appConfig?.ListenerConfig ?? throw NoConfig();
    public AppConfig.RateLimitingSection LimiterPolicyConfig => _appConfig?.LimiterPolicyConfig ?? throw NoConfig();

    private static InvalidOperationException NoConfig() =>
        new("Configuration not loaded. Ensure that RunForever has been called.");

    private bool _running;
    public bool Running => _running;

    public RecvBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        EnsureNotRunning();

        _serviceConfigs.Add(configure);
        return this;
    }

    public void RunForever(string[] args)
    {
        EnsureNotRunning();

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            EnvironmentName = Utils.IsDebug ? "Development" : "Production"
        });

        _appConfig = AppConfig.ObtainFrom(builder.Configuration) ?? throw new ArgumentException(
            "Failed to load configuration. Check if the configuration file is present and valid."
            );

        ConfigureServicesInternal(builder.Services);
        InjectDependencies(builder.Services);

        var app = builder.Build();
        var logger = app.Services.GetRequiredService<ILogger<RecvBuilder>>();

        ConfigureMiddleware(app);
        app.MapControllers();

        LogStartup(logger);

        _running = true;
        app.Run();
    }

    private void ConfigureServicesInternal(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        if (Utils.IsDebug)
        {
            services.AddSwaggerGen();
        }

        if (ListenerConfig.UseForwardedFor)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            AddRateLimiter(options, nameof(LimiterPolicyConfig.LowCost), LimiterPolicyConfig.LowCost);
            AddRateLimiter(options, nameof(LimiterPolicyConfig.MediumCost), LimiterPolicyConfig.MediumCost);
            AddRateLimiter(options, nameof(LimiterPolicyConfig.HighCost), LimiterPolicyConfig.HighCost);
        });

        services.AddControllers();
    }

    private void InjectDependencies(IServiceCollection services)
    {
        services.AddSingleton(FullConfig);
        services.AddSingleton(DatabaseConfig);
        services.AddSingleton(ListenerConfig);
        services.AddSingleton(LimiterPolicyConfig);

        foreach (var configure in _serviceConfigs)
        {
            configure(services);
        }
    }

    private void ConfigureMiddleware(WebApplication app)
    {
        if (ListenerConfig.UseForwardedFor)
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

    private void AddRateLimiter(RateLimiterOptions options, string key, RateLimitRule policy)
    {
        TimeSpan period = TimeSpan.FromSeconds(policy.Seconds);
        int limit = policy.Limit;

        options.AddPolicy(key, context =>
        {
            IPAddress? ip = context.Connection.RemoteIpAddress;

            if (ip?.IsIPv4MappedToIPv6 == true)
            {
                ip = ip.MapToIPv4();
            }

            AddressFamily family = ip?.AddressFamily ?? AddressFamily.Unknown;

            int lenV4 = Math.Clamp(ListenerConfig.PrefixIPv4, 0, 32) / 8;
            int lenV6 = Math.Clamp(ListenerConfig.PrefixIPv6, 0, 128) / 8;

            string uniqueNetworkString = family switch
            {
                AddressFamily.InterNetwork when ip is not null =>
                    Convert.ToHexString(ip.GetAddressBytes().AsSpan(0, lenV4)),

                AddressFamily.InterNetworkV6 when ip is not null =>
                    Convert.ToHexString(ip.GetAddressBytes().AsSpan(0, lenV6)),

                _ => "???"
            };

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: uniqueNetworkString,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    Window = period,
                    PermitLimit = limit,
                    QueueLimit = 0
                });
        });
    }

    private void LogStartup(ILogger logger)
    {
        logger.LogInformation("Backend is running on: {Address}", Utils.Address);

        if (Utils.IsDebug)
        {
            logger.LogInformation(
                "Swagger initialized. Open: {Address}/swagger/index.html", Utils.Address);
        }

        if (ListenerConfig.PrefixIPv4 % 8 != 0 || ListenerConfig.PrefixIPv6 % 8 != 0)
        {
            logger.LogWarning("Network prefixes should be divisible by 8 to work properly.");
        }
    }

    private void EnsureNotRunning()
    {
        if (_running)
            throw new InvalidOperationException($"{nameof(RecvBuilder)} has already been run.");
    }
}