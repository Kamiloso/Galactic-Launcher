using GalacticLauncher.Core.Records;
using System.Text.Json;
using System.Threading.RateLimiting;
using GalacticLauncher.Core.Enums;
using GalacticLauncher.Core;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;

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


        // ----- END POINTS BELOW -----
        // TODO: Make strategy pattern here

        app.MapGet("/anygame", (ILogger<Program> logger) =>
        {
            logger.LogInformation("Hello! Someone asked for game!");

            GameInfo gameInfo = new(
                "Space Eternity 3", "Big Ball of Mud", GameTag.None, [], ["xxx", "yyy"]);

            return gameInfo;
        })
        .RequireRateLimiting("AntiSpamPolicy")
        .WithName("GetTestAnyGame")
        .WithOpenApi(operation => new OpenApiOperation(operation)
        {
            Summary = "Pobiera informacje o grze",
            Description = "Ten endpoint zwraca testowe dane o grze Space Eternity 3 w formacie JSON.",
        });

        app.Run();
    }
}
