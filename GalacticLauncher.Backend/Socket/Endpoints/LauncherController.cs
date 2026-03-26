using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using GalacticLauncher.Core;
using GalacticLauncher.Core.DbRecords;
using GalacticLauncher.Backend.Repositories;

namespace GalacticLauncher.Backend.Socket.Endpoints;

[ApiController]
[Route("launcher")]
public class LauncherController(
    ILauncherRepository repository/*, ILogger<LauncherRepository> logger*/) : ControllerBase
{
    // 1. All game information
    [HttpGet("all-games")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves a list of all games.")]
    [EndpointDescription("Returns a DataList of all games for easier deserialization.")]
    public async Task<ActionResult<IEnumerable<GameInfo>>> GetAllGames()
    {
        // TODO: Remove early return when database is implemented.
        return Ok(new[] {
            new GameInfo { Id = 1, Name = "Test Game", Description = "This is a test game." },
            new GameInfo { Id = 2, Name = "Another Game", Description = "This is another test game." }
            });

        IEnumerable<GameInfo> games = await repository.GetAllGamesAsync();
        return Ok(games);
    }

    // 2. All images based on game ID
    [HttpGet("game-images")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves all images of a game.")]
    [EndpointDescription("Requires a gameId. Returns all the images for the game.")]
    public async Task<ActionResult<IEnumerable<ImgInfo>>> GetImages([FromQuery] ulong gameId)
    {
        IEnumerable<ImgInfo> images = await repository.GetImagesByGameIdAsync(gameId);
        return Ok(images);
    }

    // 3. All versions based on game ID
    [HttpGet("game-versions")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves all versions of a game.")]
    [EndpointDescription("Requires a gameId. Returns all the versions for the game.")]
    public async Task<ActionResult<IEnumerable<VersionInfo>>> GetVersions([FromQuery] ulong gameId)
    {
        IEnumerable<VersionInfo> versions = await repository.GetVersionsByGameIdAsync(gameId);
        return Ok(versions);
    }

    // 4. Primary version based on game ID
    [HttpGet("game-versions/primary")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves the primary version of a game.")]
    [EndpointDescription("Requires a gameId. Returns the primary version for a game.")]
    public async Task<ActionResult<VersionInfo>> GetPrimaryVersion([FromQuery] ulong gameId)
    {
        VersionInfo? version = await repository.GetPrimaryVersionAsync(gameId);
        return version is not null ? Ok(version) : NotFound();
    }

    // 5. Find executables based on version ID
    [HttpGet("version-execs")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves executables for a version.")]
    [EndpointDescription("Requires a versionId. Returns the executable info for a certain version.")]
    public async Task<ActionResult<IEnumerable<ExecInfo>>> GetExecs([FromQuery] ulong versionId)
    {
        IEnumerable<ExecInfo> execs = await repository.GetExecsByVersionIdAsync(versionId);
        return Ok(execs);
    }
}