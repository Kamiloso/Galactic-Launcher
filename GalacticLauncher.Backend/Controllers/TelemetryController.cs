using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Backend.Services;
using GalacticLauncher.Core.Dto;

namespace GalacticLauncher.Backend.Controllers;

[ApiController]
[Route("telemetry")]
public class TelemetryController(
    ILogger<TelemetryController> logger,
    IHistoryService historyService) : ControllerBack(logger, historyService)
{
    [HttpPost("play-game")]
    [EnableRateLimiting("TelemetryCost")]
    [EndpointDescription("Registers information when player starts to play the game.")]
    public ActionResult GameEcho(
        [FromBody] TelemetryBox<PlayGame> telemetryBox)
    {
        PlayGame body = telemetryBox.Body.Sanitize();

        LogAuto(new { telemetryBox.Guid, PlayGame = body },
            importance: LogLevel.Information,
            toHistory: true,
            idGame: body.GameId);

        return HandleEndpoint(() => { });
    }
}
