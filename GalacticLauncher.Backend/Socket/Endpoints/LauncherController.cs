using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using GalacticLauncher.Core;
using GalacticLauncher.Core.DbRecords;
using GalacticLauncher.Backend.Database;
using GalacticLauncher.Backend.Repositories;
using GalacticLauncher.Backend.Responses;

namespace GalacticLauncher.Backend.Socket.Endpoints;

[ApiController]
[Route("games")] // TODO: Should be changed 
public class LauncherController(ILauncherRepository repository, ILogger<LauncherRepository> logger) : ControllerBase
{
    // 1. All game information
    [HttpGet("all")]
    [EnableRateLimiting(Utils.LOW_COST)]
    [EndpointSummary("Retrieves a list of all games.")]
    [EndpointDescription("Returns a DataList of all games for easier deserialization.")]
    public async Task<ActionResult<DataList<GameInfo>>> GetAllGames()
    {
        var games = await repository.GetAllGamesAsync();
        return Ok(new DataList<GameInfo>(games));
    }

    // 2. All images based on game ID
    [HttpGet("images")]
    [EnableRateLimiting(Utils.LOW_COST)]
    [EndpointSummary("Retrieves all images of a game.")]
    [EndpointDescription("Requires a gameId. Returns all the images for the game.")]
    public async Task<ActionResult<DataList<ImgInfo>>> GetImages([FromQuery] ulong gameId)
    {
        var images = await repository.GetImagesByGameIdAsync(gameId);
        return Ok(new DataList<ImgInfo>(images));
    }

    // 3. All versions based on game ID
    [HttpGet("versions")]
    [EnableRateLimiting(Utils.LOW_COST)]
    [EndpointSummary("Retrieves all versions of a game.")]
    [EndpointDescription("Requires a gameId. Returns all the images for the game.")]
    public async Task<ActionResult<DataList<VersionInfo>>> GetVersions([FromQuery] ulong gameId)
    {
        var versions = await repository.GetVersionsByGameIdAsync(gameId);
        return Ok(new DataList<VersionInfo>(versions));
    }

    // 4. Primary version based on game ID
    [HttpGet("versions/primary")]
    [EnableRateLimiting(Utils.LOW_COST)]
    [EndpointSummary("Retrieves the primary version of a game.")]
    [EndpointDescription("Requires a gameId. Returns the primary version for a game.")]
    public async Task<ActionResult<VersionInfo>> GetPrimaryVersion([FromQuery] ulong gameId)
    {
        var version = await repository.GetPrimaryVersionAsync(gameId);
        return version is not null ? Ok(version) : NotFound();
    }

    // 5. Find executables based on version ID
    [HttpGet("execs")]
    [EnableRateLimiting(Utils.LOW_COST)]
    [EndpointSummary("Retrieves executables for a version.")]
    [EndpointDescription("Requires a versionId. Returns the executable info for a certain version.")]
    public async Task<ActionResult<DataList<ExecInfo>>> GetExecs([FromQuery] ulong versionId)
    {
        var execs = await repository.GetExecsByVersionIdAsync(versionId);
        return Ok(new DataList<ExecInfo>(execs));
    }
}