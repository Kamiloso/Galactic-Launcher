using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Backend.Controllers;

[ApiController]
[Route("testing")]
public class TestingController(
    ILogger<TestingController> logger) : ControllerBack(logger)
{
    [HttpPost("game-echo")]
    [EnableRateLimiting("LowCost")]
    [EndpointDescription("It returns provided game to you.")]
    public ActionResult<Game> GameEcho(
        [FromBody] Game game)
    {
        LogCallToConsole();

        return HandleEndpoint(
            () => game);
    }
}