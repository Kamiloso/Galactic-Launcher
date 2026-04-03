using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using GalacticLauncher.Core.DbRecords;
using GalacticLauncher.Backend.Repositories;

namespace GalacticLauncher.Backend.Endpoints;

[ApiController]
[Route("launcher")]
public class LauncherController(
    IGameRepository gameRepo,
    IImageRepository imageRepo,
    IVersionRepository versionRepo,
    IExecRepository execRepo,
    ILogger<LauncherController> logger
    ) : ControllerBase
{
    // 1. All game information
    [HttpGet("all-games")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves a list of all games.")]
    [EndpointDescription("Returns a DataList of all games for easier deserialization.")]
    public async Task<ActionResult<IEnumerable<GameInfo>>> GetAllGames()
    {
        IEnumerable<GameInfo> allGames = await gameRepo.GetAllGames();
        return Ok(allGames);
    }

    // 2. All images based on game ID
    [HttpGet("game-images")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves all images of a game.")]
    [EndpointDescription("Requires a gameId. Returns all the images for the game.")]
    public async Task<ActionResult<IEnumerable<ImageInfo>>> GetImages([FromQuery] ulong gameId)
    {
        logger.LogDebug($"Received request: {nameof(GetImages)} with gameId={{gameId}}", gameId);
        IEnumerable<ImageInfo> images = await imageRepo.GetImagesByGameId(gameId);
        return Ok(images);
    }

    // 3. All versions based on game ID
    [HttpGet("game-versions")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves all versions of a game.")]
    [EndpointDescription("Requires a gameId. Returns all the versions for the game.")]
    public async Task<ActionResult<IEnumerable<VersionInfo>>> GetVersions([FromQuery] ulong gameId)
    {
        logger.LogDebug($"Received request: {nameof(GetVersions)} with gameId={{gameId}}", gameId);
        IEnumerable<VersionInfo> versions = await versionRepo.GetVersionsByGameId(gameId);
        return Ok(versions);
    }

    // 4. Primary version based on game ID
    [HttpGet("game-versions/primary")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves the primary version of a game.")]
    [EndpointDescription("Requires a gameId. Returns the primary version for a game.")]
    public async Task<ActionResult<VersionInfo>> GetPrimaryVersion([FromQuery] ulong gameId)
    {
        logger.LogDebug($"Received request: {nameof(GetPrimaryVersion)} with gameId={{gameId}}", gameId);
        VersionInfo? version = await versionRepo.GetPrimaryVersion(gameId);
        return version is not null ? Ok(version) : NotFound();
    }

    // 5. Find executables based on version ID
    [HttpGet("version-execs")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves executables for a version.")]
    [EndpointDescription("Requires a versionId. Returns the executable info for a certain version.")]
    public async Task<ActionResult<IEnumerable<ExecInfo>>> GetExecs([FromQuery] ulong versionId)
    {
        logger.LogDebug($"Received request: {nameof(GetExecs)} with versionId={{versionId}}", versionId);
        IEnumerable<ExecInfo> execs = await execRepo.GetExecsByVersionId(versionId);
        return Ok(execs);
    }
}