using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Backend.Services;
using GalacticLauncher.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GalacticLauncher.Backend.Controllers;

[ApiController]
[Route("download")]
public class DownloadController(
    ILogger<DownloadController> logger,
    IHistoryService historyService,
    IDataAccessService dataAccessService) : ControllerBack(logger, historyService)
{
    [HttpGet("all-games")]
    [EnableRateLimiting("MediumCost")]
    [EndpointDescription("Returns a list of all games.")]
    public async Task<ActionResult<IEnumerable<Game>>> AllGames()
    {
        LogAuto();

        return await HandleEndpointAsync(
            () => dataAccessService.GetAllGames());
    }

    [HttpGet("game-data")]
    [EnableRateLimiting("MediumCost")]
    [EndpointDescription("Returns detailed information about a specific game.")]
    public async Task<ActionResult<GameData>> GameData(
        [FromQuery] long id)
    {
        LogAuto(new { Id = id });

        return await HandleEndpointAsync(
            () => dataAccessService.GetGameDataById(id));
    }

    [HttpGet("all-tags")]
    [EnableRateLimiting("MediumCost")]
    [EndpointDescription("Returns a list of all currently existing tags.")]
    public async Task<ActionResult<IEnumerable<Tag>>> AllTags()
    {
        LogAuto();

        return await HandleEndpointAsync(
            () => dataAccessService.GetAllTags());
    }
}
