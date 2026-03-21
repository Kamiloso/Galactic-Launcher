using GalacticLauncher.Backend.Interfaces;
using GalacticLauncher.Core.Enums;
using GalacticLauncher.Core.Records;
using Microsoft.OpenApi.Models;

namespace GalacticLauncher.Backend.Endpoints
{
    public class AnyGameEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/anygame", HandleRequest)
                .RequireRateLimiting("AntiSpamPolicy")
                .WithName("GetTestAnyGame")
                .WithOpenApi(operation => new OpenApiOperation(operation)
                {
                    Summary = "Pobiera informacje o grze",
                    Description = "Ten endpoint zwraca testowe dane o grze Space Eternity 3 w formacie JSON."
                });
        }

        private static GameInfo HandleRequest(ILogger<AnyGameEndpoint> logger)
        {
            logger.LogInformation("Hello! Someone asked for game!");

            return new GameInfo(
                "Space Eternity 3", "Big Ball of Mud", GameTag.None, [], ["xxx", "yyy"]
            );
        }
    }
}