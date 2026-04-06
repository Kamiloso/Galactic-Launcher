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
    ITagRepository tagRepo,
    IUserRepository userRepo,
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
    
    // 2. All tag information
    [HttpGet("all-tags")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves a list of all tags.")]
    [EndpointDescription("Returns every available tag in the database.")]
    public async Task<ActionResult<IEnumerable<TagInfo>>> GetAllTags()
    {
        IEnumerable<TagInfo> tags = await tagRepo.GetAllTags();
        return Ok(tags);
    }
    

    // 3. All images based on game ID
    [HttpGet("game-images")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves all images of a game.")]
    [EndpointDescription("Requires a gameId. Returns all the images for the game.")]
    public async Task<ActionResult<IEnumerable<ImageInfo>>> GetImages([FromQuery] long gameId)
    {
        logger.LogDebug($"Received request: {nameof(GetImages)} with gameId={{gameId}}", gameId);
        IEnumerable<ImageInfo> images = await imageRepo.GetImagesByGameId(gameId);
        return Ok(images);
    }

    // 4. All versions based on game ID
    [HttpGet("game-versions")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves all versions of a game.")]
    [EndpointDescription("Requires a gameId. Returns all the versions for the game.")]
    public async Task<ActionResult<IEnumerable<VersionInfo>>> GetVersions([FromQuery] long gameId)
    {
        logger.LogDebug($"Received request: {nameof(GetVersions)} with gameId={{gameId}}", gameId);
        IEnumerable<VersionInfo> versions = await versionRepo.GetVersionsByGameId(gameId);
        return Ok(versions);
    }

    // 5. Primary version based on game ID
    [HttpGet("game-versions/primary")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves the primary version of a game.")]
    [EndpointDescription("Requires a gameId. Returns the primary version for a game.")]
    public async Task<ActionResult<VersionInfo>> GetPrimaryVersion([FromQuery] long gameId)
    {
        logger.LogDebug($"Received request: {nameof(GetPrimaryVersion)} with gameId={{gameId}}", gameId);
        VersionInfo? version = await versionRepo.GetPrimaryVersion(gameId);
        return version is not null ? Ok(version) : NotFound();
    }

    // 6. Find executables based on version ID
    [HttpGet("version-execs")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves executables for a version.")]
    [EndpointDescription("Requires a versionId. Returns the executable info for a certain version.")]
    public async Task<ActionResult<IEnumerable<ExecInfo>>> GetExecs([FromQuery] long versionId)
    {
        logger.LogDebug($"Received request: {nameof(GetExecs)} with versionId={{versionId}}", versionId);
        IEnumerable<ExecInfo> execs = await execRepo.GetExecsByVersionId(versionId);
        return Ok(execs);
    }

    // 7. All tags based on game ID
    [HttpGet("game-tags")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves all tags for a specific game.")]
    [EndpointDescription("Requires a gameId. Returns the full tag list for the game.")]
    public async Task<ActionResult<IEnumerable<TagInfo>>> GetGameTags([FromQuery] long gameId)
    {
        logger.LogDebug($"Received request: {nameof(GetGameTags)} with gameId={{gameId}}", gameId);
        IEnumerable<TagInfo> tags = await tagRepo.GetTagsByGameId(gameId);
        return Ok(tags);
    }
    
    // 8. All games based on tag ID
    [HttpGet("tag-games")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves all games for a specific tag.")]
    [EndpointDescription("Requires a tagId. Returns the full game list for the tag.")]
    public async Task<ActionResult<IEnumerable<GameInfo>>> GetTagGames([FromQuery] long tagId)
    {
        logger.LogDebug($"Received request: {nameof(GetTagGames)} with tagId={{tagId}}", tagId);
        IEnumerable<GameInfo> games = await gameRepo.GetGamesByTagId(tagId);
        return Ok(games);
    }

    // 9. Find user based on email
    [HttpGet("user/by-email")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves a user by their email address.")]
    [EndpointDescription("Requires an email string. Returns user info if found.")]
    public async Task<ActionResult<UserInfo>> GetUserByEmail([FromQuery] string email)
    {
        logger.LogDebug($"Received request: {nameof(GetUserByEmail)} with email={{email}}", email);
        UserInfo? user = await userRepo.GetUserByEmail(email);
        return user is not null ? Ok(user) : NotFound();
    }

    // 10. Find user based on Google Key
    [HttpGet("user/by-google")]
    [EnableRateLimiting("LowCost")]
    [EndpointSummary("Retrieves a user by their Google Auth Key.")]
    [EndpointDescription("Requires a googleKey string. Returns user info if found.")]
    public async Task<ActionResult<UserInfo>> GetUserByGoogleKey([FromQuery] string googleKey)
    {
        logger.LogDebug($"Received request: {nameof(GetUserByGoogleKey)} with googleKey={{googleKey}}", googleKey);
        UserInfo? user = await userRepo.GetUserByGoogleKey(googleKey);
        return user is not null ? Ok(user) : NotFound();
    }
}