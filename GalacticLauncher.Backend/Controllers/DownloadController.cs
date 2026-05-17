using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Backend.Services;

namespace GalacticLauncher.Backend.Controllers;

[ApiController]
[Route("download")]
public class DownloadController(
    ILogger<DownloadController> logger,
    IDataAccessService dataAccessService
    ) : ControllerBack(logger)
{
    [HttpGet("game-info")]
    [EnableRateLimiting("MediumCost")]
    [EndpointSummary("Returns detailed information about a game by its id.")]
    public async Task<ActionResult<GameData>> GameInfo(
        [FromQuery] long id)
    {
        LogCallToConsole(new { id });

        GameData result =
            await dataAccessService.GetGameDataById(id);

        return Ok(result);
    }

    [HttpGet("all-games")]
    [EnableRateLimiting("MediumCost")]
    [EndpointSummary("Returns a list of all games basic information.")]
    public async Task<ActionResult<IEnumerable<Game>>> AllGames()
    {
        LogCallToConsole();

        IEnumerable<Game> result =
            await dataAccessService.GetAllGames();

        return Ok(result);
    }

    [HttpGet("all-tags")]
    [EnableRateLimiting("MediumCost")]
    [EndpointSummary("Returns a list of all existing tags.")]
    public async Task<ActionResult<IEnumerable<Tag>>> AllTags()
    {
        LogCallToConsole();

        IEnumerable<Tag> result =
            await dataAccessService.GetAllTags();

        return Ok(result);
    }

    [HttpPost("games-by-tags")]
    [EnableRateLimiting("HighCost")]
    [EndpointSummary("Returns a list of games basic information with all of the given tags.")]
    public async Task<ActionResult<Game>> GameInfo(
        [FromBody] IEnumerable<long> tagIds)
    {
        LogCallToConsole(new { tagIds });

        if (tagIds.Count() > 10)
            return BadRequest("Too many tags. Maximum allowed is 10.");

        IEnumerable<Game> result =
            await dataAccessService.GetGamesByTagIds(tagIds);

        return Ok(result);
    }
}
