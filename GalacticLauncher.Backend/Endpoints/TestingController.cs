using Microsoft.AspNetCore.Mvc;
using GalacticLauncher.Core.DbRecords;
using GalacticLauncher.Core;
using Microsoft.AspNetCore.RateLimiting;

namespace GalacticLauncher.Backend.Endpoints;

[ApiController]
[Route("testing")]
public class TestingController(ILogger<TestingController> Logger) : ControllerBase
{
    [HttpPost("game-echo")] // may be GET as well (prefer GET), but POST gives more possibilities
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Returns sent game.")]
    [EndpointDescription("This is a robust description...")]
    public ActionResult<GameInfo> GameEcho([FromBody] GameInfo gameInfo)
    {
        if (Utils.IsProduction)
        {
            return NotFound();
        }

        Logger.LogInformation("GameEcho called with: {GameInfo}", gameInfo);
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