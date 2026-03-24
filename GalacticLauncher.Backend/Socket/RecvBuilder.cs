using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;
using System.Net.Sockets;
using System.Threading.RateLimiting;

namespace GalacticLauncher.Backend.Socket;

internal sealed class RecvBuilder(string? Address = null)
{
    private record LimiterPolicy(TimeSpan Period, int Limit);

    private readonly List<Action<IServiceCollection>> _serviceConfigurations = [];
    private readonly Dictionary<string, LimiterPolicy> _rateLimits = [];

    private bool _running;

    public int PrefixIPv4 { get; private set; } = 32;
    public int PrefixIPv6 { get; private set; } = 56;
    public bool UseForwardedFor { get; private set; } = false;

    public bool Running => _running;
    public bool SwaggerEnabled { get; private set; }

    public RecvBuilder SetConfig(int prefixIPv4, int prefixIPv6, bool useForwardedFor)
    {
        EnsureNotRunning();
        PrefixIPv4 = prefixIPv4;
        PrefixIPv6 = prefixIPv6;
        UseForwardedFor = useForwardedFor;
        return this;
    }

    public RecvBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        EnsureNotRunning();
        _serviceConfigurations.Add(configure);
        return this;
    }

    public RecvBuilder WithRateLimit(string key, TimeSpan period, int limit)
    {
        EnsureNotRunning();
        _rateLimits[key] = new LimiterPolicy(period, limit);
        return this;
    }

    public void RunForever()
    {
        EnsureNotRunning();

        var builder = WebApplication.CreateBuilder();

        ConfigureServicesInternal(builder.Services);

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

#if DEBUG
        services.AddSwaggerGen();
#endif

        if (UseForwardedFor)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        if (_rateLimits.Count > 0)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                foreach (var (key, policy) in _rateLimits)
                {
                    AddRateLimiter(options, key, policy);
                }
            });
        }

        services.AddControllers();

        foreach (var configure in _serviceConfigurations)
        {
            configure(services);
        }
    }

    private void ConfigureMiddleware(WebApplication app)
    {
        if (UseForwardedFor)
        {
            app.UseForwardedHeaders();
        }

#if DEBUG
        app.UseSwagger();
        app.UseSwaggerUI();
        SwaggerEnabled = true;
#endif

        app.UseHttpsRedirection();

        if (_rateLimits.Count > 0)
        {
            app.UseRateLimiter();
        }
    }

    private void LogStartup(ILogger logger)
    {
        logger.LogInformation("Backend is running on: {Address}", Address ?? "???");

        if (SwaggerEnabled)
        {
            logger.LogInformation(
                "Swagger initialized. Open: {Address}/swagger/index.html", Address ?? "???");
        }

        if (PrefixIPv4 % 8 != 0 || PrefixIPv6 % 8 != 0)
        {
            logger.LogWarning("Network prefixes should be divisible by 8 to work properly.");
        }
    }

    private void AddRateLimiter(RateLimiterOptions options, string key, LimiterPolicy policy)
    {
        var (period, limit) = policy;

        options.AddPolicy(key, context =>
        {
            IPAddress? ip = context.Connection.RemoteIpAddress;
            AddressFamily family = ip?.AddressFamily ?? AddressFamily.Unknown;

            int lenV4 = Math.Clamp(PrefixIPv4, 0, 32) / 8;
            int lenV6 = Math.Clamp(PrefixIPv6, 0, 128) / 8;

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

    private void EnsureNotRunning()
    {
        if (_running)
            throw new InvalidOperationException("RecvBuilder has already been run.");
    }
}