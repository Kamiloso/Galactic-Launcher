using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using GalacticLauncher.Backend.Repositories;
using GalacticLauncher.Backend.Patches;

namespace GalacticLauncher.Backend.Endpoints;

[ApiController]
[Route("download")]
public class DownloadController(
    ILogger<DownloadController> logger,
    IGameRepository gameRepository
    ) : ControllerBack(logger)
{
    [HttpGet("all-games")]
    [EnableRateLimiting("MediumCost")]
    [EndpointSummary("Returns a list of all basic game information.")]
    public async Task<ActionResult<IEnumerable<long>>> AllGames()
    {
        LogCallToConsole();

        return Ok(
            await gameRepository.GetAllGameIds()
            );
    }
}