using Microsoft.AspNetCore.Mvc;
using GalacticLauncher.Core;
using Microsoft.AspNetCore.RateLimiting;
using GalacticLauncher.Core.DbModels;
using GalacticLauncher.Backend.Patches;

namespace GalacticLauncher.Backend.Endpoints;

[ApiController]
[Route("testing")]
public class TestingController(
    ILogger<TestingController> logger
    ) : ControllerBack(logger)
{
    [HttpPost("game-echo")] // may be GET as well (prefer GET), but POST gives more possibilities
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("It's like echo. Send it a game and it will return it to you.")]
    [EndpointDescription("This is a robust description...")]
    public async Task<ActionResult<Game>> GameEcho(
        [FromBody] Game gameInfo)
    {
        if (Utils.IsProduction)
            return NotFound(); // ignore in production

        LogCallToConsole();

        return Ok(gameInfo);
    }

    // ---------------------------------------------------
    // may copy-paste more endpoints but may also copy the whole class
    // do as you wish, it should be practical and clean
    // ---------------------------------------------------
    // also do not put any advanced computations here
    // only endpoint handling, SRP
    // ---------------------------------------------------
}