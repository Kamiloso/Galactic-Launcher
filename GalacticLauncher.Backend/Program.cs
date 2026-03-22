using GalacticLauncher.Backend.Endpoints;
using GalacticLauncher.Core;
using GalacticLauncher.Core.Enums;
using GalacticLauncher.Core.Records;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Threading.RateLimiting;

namespace GalacticLauncher.Backend;

class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy("AntiSpamPolicy", context =>
            {
                var ip = context.Connection.RemoteIpAddress;
                string partitionKey = "unknown_ip";

                if (ip != null)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        var bytes = ip.GetAddressBytes();
                        partitionKey = BitConverter.ToString(bytes, 0, 8);
                    }
                    else
                    {
                        partitionKey = ip.ToString();
                    }
                }

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: partitionKey,
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromSeconds(5),
                        QueueLimit = 0
                    });
            });
        });

        builder.Services.AddEndpointsApiExplorer();

#if DEBUG
        builder.Services.AddSwaggerGen();
#endif

        var app = builder.Build();
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation(
            "Backend is running on: {Address}", Utils.Address
            );

#if DEBUG
        app.UseSwagger();
        app.UseSwaggerUI();
        logger.LogInformation(
            "Swagger has been initialized. Open: {Address}/swagger/index.html", Utils.Address
            );
#endif

        app.UseRateLimiter();
        app.UseHttpsRedirection();
        
        app.MapAllEndpoints();

        app.Run();

    }
}
