using GalacticLauncher.Backend.Domain.Exceptions;
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
    IDataAccessService dataAccessService) : ControllerBack(logger)
{
    [HttpGet("all-games")]
    [EnableRateLimiting("MediumCost")]
    [EndpointDescription("Returns a list of all games.")]
    public async Task<ActionResult<IEnumerable<Game>>> AllGames()
    {
        LogCallToConsole();

        return await HandleEndpointAsync(
            () => dataAccessService.GetAllGames());
    }

    [HttpGet("game-data")]
    [EnableRateLimiting("MediumCost")]
    [EndpointDescription("Returns detailed information about a specific game.")]
    public async Task<ActionResult<GameData>> GameData(
        [FromQuery] long id)
    {
        LogCallToConsole(new { id });

        return await HandleEndpointAsync(
            () => dataAccessService.GetGameDataById(id));
    }

    [HttpGet("all-tags")]
    [EnableRateLimiting("MediumCost")]
    [EndpointDescription("Returns a list of all currently existing tags.")]
    public async Task<ActionResult<IEnumerable<Tag>>> AllTags()
    {
        LogCallToConsole();

        return await HandleEndpointAsync(
            () => dataAccessService.GetAllTags());
    }

    [HttpPost("games-by-tags")]
    [EnableRateLimiting("MediumCost")]
    [EndpointDescription("Returns a list of all games that contain all of the provided tags.")]
    public async Task<ActionResult<IEnumerable<Game>>> GamesByTags(
        [FromBody] IEnumerable<long> tagIds)
    {
        LogCallToConsole(new { tagIds });

        return await HandleEndpointAsync(() =>
        {
            if (tagIds.Count() > 16)
                throw ClientFaultException.BadRequest400("Too many tags provided. Max 16 allowed.");

            return dataAccessService.GetGamesByTagIds(tagIds);
        });
    }
}
