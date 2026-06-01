using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Backend.Services;

namespace GalacticLauncher.Backend.Controllers;

[ApiController]
[Route("testing")]
public class TestingController(
    ILogger<DownloadController> logger,
    IHistoryService historyService) : ControllerBack(logger, historyService)
{
    [HttpPost("game-echo")]
    [EnableRateLimiting("LowCost")]
    [EndpointDescription("It returns provided game to you.")]
    public ActionResult<Game> GameEcho(
        [FromBody] Game game)
    {
        LogAuto(game);

        return HandleEndpoint(() => game);
    }
}