using GalacticLauncher.Core.Records;
using System.Text.Json;
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

        // TODO: Create rate limiter

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

        // app.UseRateLimiter();
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
        .WithName("GetTestAnyGame")
        .WithOpenApi(operation => new OpenApiOperation(operation)
        {
            Summary = "Pobiera informacje o grze",
            Description = "Ten endpoint zwraca testowe dane o grze Space Eternity 3 w formacie JSON.",
        });

        app.Run();
    }
}
