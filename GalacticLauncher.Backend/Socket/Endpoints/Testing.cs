using Microsoft.AspNetCore.Mvc;
using GalacticLauncher.Core.DbRecords;
using GalacticLauncher.Core;
using Microsoft.AspNetCore.RateLimiting;

namespace GalacticLauncher.Backend.Socket.Endpoints;

[ApiController]
[Route("testing")]
public class Testing(ILogger<Testing> Logger) : ControllerBase
{
    [HttpPost("game-echo")] // may be GET as well (prefer GET), but POST gives more possibilities
    [EnableRateLimiting(Utils.LOW_COST)]
    [EndpointSummary("Returns sent game.")]
    [EndpointDescription("This is a robust description...")]
    public ActionResult<GameInfo> GameEcho([FromBody] GameInfo gameInfo)
    {
        Logger.LogInformation("Returned game.");
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