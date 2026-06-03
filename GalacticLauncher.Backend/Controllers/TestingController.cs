using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Backend.Services;
using GalacticLauncher.Core;
using GalacticLauncher.Backend.Domain.Exceptions;

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

    [HttpGet("get-error")]
    [EnableRateLimiting("MediumCost")]
    [EndpointDescription("It throws an exception to test error handling.")]
    public ActionResult GetError()
    {
        LogAuto();

        return HandleEndpoint(
            () =>
            {
                throw Utils.IsProduction
                    ? ClientFaultException.BadRequest400("This endpoint is only available in development environment.")
                    : new Exception("This is a simulated server error.");
            });
    }
}
