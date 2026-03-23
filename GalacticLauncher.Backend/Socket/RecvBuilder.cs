using GalacticLauncher.Core;
using System.Net.Sockets;
using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;

namespace GalacticLauncher.Backend.Socket;

internal class RecvBuilder
{
    private enum Stage
    {
        Created = 0,
        RatesLimited = 1,
        DepInjected = 2,
        AppBuilt = 3,
        RegisteredEps = 4,
        Running = 5,
    }

    public int PrefixIPv4 { get; init; } = 32;
    public int PrefixIPv6 { get; init; } = 56;
    public bool UseForwardedFor { get; init; } = false;

    public bool Running => _stage == Stage.Running;
    public bool SwaggerEnabled { get; private set; } = false;

    private readonly WebApplicationBuilder _builder;
    private WebApplication? _app;
    private ILogger? _logger;
    private Stage _stage;

    public RecvBuilder()
    {
        _builder = WebApplication.CreateBuilder();
        _stage = Stage.Created;
    }

    private void IncreaseStageTo(Stage target)
    {
        if (_stage + 1 != target)
            throw new InvalidOperationException("Invalid stage!");

        _stage++;
    }

    public RecvBuilder AddRateLimiters(Dictionary<string, Policy?> policies)
    {
        IncreaseStageTo(Stage.RatesLimited);

        _builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            foreach (var (key, policy) in policies)
            {
                if (policy != null)
                {
                    AddRateLimiter(options, key, policy);
                }
            }
        });

        return this;
    }

    private void AddRateLimiter(RateLimiterOptions options, string key, Policy policy)
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
                AddressFamily.InterNetwork =>
                    Convert.ToHexString(ip!.GetAddressBytes().AsSpan(0, lenV4)),

                AddressFamily.InterNetworkV6 =>
                    Convert.ToHexString(ip!.GetAddressBytes().AsSpan(0, lenV6)),

                _ => "???"
            };

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: uniqueNetworkString,
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    Window = period,
                    PermitLimit = limit,
                    QueueLimit = 0
                });
        });
    }

    public RecvBuilder InjectDependencies(Action<IServiceCollection> inject)
    {
        IncreaseStageTo(Stage.DepInjected);

        inject(_builder.Services);

        return this;
    }

    public RecvBuilder BuildWebApp()
    {
        IncreaseStageTo(Stage.AppBuilt);

        _builder.Services.AddEndpointsApiExplorer();
#if DEBUG
        _builder.Services.AddSwaggerGen();
#endif
        if (UseForwardedFor)
        {
            _builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        _app = _builder.Build();
        _logger = _app.Services.GetRequiredService<ILogger<RecvBuilder>>();

        if (UseForwardedFor)
        {
            _app.UseForwardedHeaders();
        }

#if DEBUG
        _app.UseSwagger();
        _app.UseSwaggerUI();
        SwaggerEnabled = true;
#endif

        _app.UseHttpsRedirection();
        _app.UseRateLimiter();

        return this;
    }

    public RecvBuilder RegisterEndpoints(Action<WebApplication> register)
    {
        IncreaseStageTo(Stage.RegisteredEps);

        register(_app!);

        return this;
    }

    public RecvBuilder Run()
    {
        IncreaseStageTo(Stage.Running);

        _logger!.LogInformation(
            "Backend is running on: {Address}", Utils.Address
            );

        if (SwaggerEnabled)
        {
            _logger!.LogInformation(
                "Swagger initialized. Open: {Address}/swagger/index.html", Utils.Address
                );
        }

        if (PrefixIPv4 % 8 != 0 ||  PrefixIPv6 % 8 != 0)
        {
            _logger!.LogWarning(
                "Network prefixes should be divisible by 8 to work properly."
                );
        }

        _app!.Run();

        return this;
    }
}
