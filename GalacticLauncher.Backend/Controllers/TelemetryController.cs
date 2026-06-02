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
        [FromBody] Telemetry<PlayGame> telemetryBox)
    {
        LogAuto(new { telemetryBox.Guid, PlayGame = telemetryBox.Body },
            importance: LogLevel.Information,
            toHistory: true,
            idGame: telemetryBox.Body.Game.Id);

        return HandleEndpoint(() => { });
    }
}
