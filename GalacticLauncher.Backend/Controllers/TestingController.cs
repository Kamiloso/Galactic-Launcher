using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Backend.Controllers;

[ApiController]
[Route("testing")]
public class TestingController(
    ILogger<TestingController> logger
    ) : ControllerBack(logger)
{
    [HttpPost("game-echo")] // may be GET as well (prefer GET), but POST gives more possibilities
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("I think it's self-explanatory.")]
    [EndpointDescription("This is a robust description...")]
    public async Task<ActionResult<Game>> GameEcho(
        [FromBody] Game game)
    {
        LogCallToConsole();

        return Ok(game);
    }

    // ---------------------------------------------------
    // may copy-paste more endpoints but may also copy the whole class
    // do as you wish, it should be practical and clean
    // ---------------------------------------------------
    // also do not put any advanced computations here
    // only endpoint handling, SRP
    // ---------------------------------------------------
}