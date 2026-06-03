#pragma warning disable CS9107
using GalacticLauncher.Backend.Domain.Exceptions;
using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Backend.Services;
using GalacticLauncher.Core.Dto;
using GalacticLauncher.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GalacticLauncher.Backend.Controllers;

[ApiController]
[Route("admin")]
public class AdminController(
    ILogger<AdminController> logger,
    IHistoryService historyService,
    IAdminService adminService,
    IDataUpdateService dataUpdateService) : ControllerBack(logger, historyService)
{
    [HttpPost("req-admin")]
    [EnableRateLimiting("ReqCost")]
    [EndpointDescription("Asks the server for the admin token.")]
    public ActionResult<LoginResult> ReqAdmin(
        [FromBody] LoginRequest loginRequest)
    {
        LogAuto(new { loginRequest.Username, Password = "*****" },
            importance: LogLevel.Information,
            toHistory: true);

        return HandleEndpoint(() =>
        {
            LoginResult result = adminService.AuthenticateAdmin(loginRequest);

            if (result.Authenticated)
            {
                LogAuto(new { loginRequest.Username },
                    verb: "authenticated using",
                    importance: LogLevel.Information,
                    toHistory: true);
            }

            return result;
        });
    }

    private void EnsureValidToken(string token, out string username)
    {
        if (!adminService.TryValidateSession(token, out username))
            throw ClientFaultException.Unauthorized401("Invalid or expired admin token.");
    }

    [HttpPost("update-game-tree")]
    [EnableRateLimiting("HighCost")]
    [EndpointDescription("Updates the whole game tree in the database.")]
    public async Task<ActionResult> UpdateGameData(
        [FromBody] AdminBox<GameTree> adminBox)
    {
        return await HandleEndpointAsync(() =>
        {
            EnsureValidToken(adminBox.Token, out string username);

            LogAuto(new { Username = username, GameData = adminBox.Body },
                toHistory: true,
                idGame: adminBox.Body.Id);

            return dataUpdateService.UpdateGameTree(adminBox.Body);
        });
    }

    [HttpPost("create-game")]
    [EnableRateLimiting("MediumCost")]
    [EndpointDescription("Creates a new game and adds it to the database.")]
    public async Task<ActionResult<long>> CreateGame(
        [FromBody] AdminBox<Game> adminBox)
    {
        return await HandleEndpointAsync(async () =>
        {
            EnsureValidToken(adminBox.Token, out string username);

            long? idGame = null;

            try
            {
                // id is needed for logging -> logging after creation
                // try-finally to ensure we log even if creation fails

                idGame = await dataUpdateService.CreateGame(adminBox.Body);
                return idGame.Value;
            }
            finally
            {
                LogAuto(new { Username = username, Game = adminBox.Body },
                    toHistory: true,
                    idGame: idGame);
            }
        });
    }

    [HttpPost("delete-game")]
    [EnableRateLimiting("MediumCost")]
    [EndpointDescription("Deletes a game from the database.")]
    public async Task<ActionResult> DeleteGame(
        [FromBody] AdminBox adminBox,
        [FromQuery] long id)
    {
        return await HandleEndpointAsync(() =>
        {
            EnsureValidToken(adminBox.Token, out string username);

            LogAuto(new { Username = username, Id = id },
                toHistory: true,
                idGame: id);

            return dataUpdateService.DeleteGameById(id);
        });
    }

    [HttpPost("create-tag")]
    [EnableRateLimiting("MediumCost")]
    [EndpointDescription("Creates a new tag and adds it to the database.")]
    public async Task<ActionResult<long>> CreateTag(
        [FromBody] AdminBox<Tag> adminBox)
    {
        return await HandleEndpointAsync(() =>
        {
            EnsureValidToken(adminBox.Token, out string username);

            LogAuto(new { Username = username, Tag = adminBox.Body },
                toHistory: true);

            return dataUpdateService.CreateTag(adminBox.Body);
        });
    }

    [HttpPost("delete-tag")]
    [EnableRateLimiting("MediumCost")]
    [EndpointDescription("Deletes a tag from the database.")]
    public async Task<ActionResult> DeleteTag(
        [FromBody] AdminBox adminBox,
        [FromQuery] long id)
    {
        return await HandleEndpointAsync(() =>
        {
            EnsureValidToken(adminBox.Token, out string username);

            LogAuto(new { Username = username, Id = id },
                toHistory: true);

            return dataUpdateService.DeleteTagById(id);
        });
    }

    [HttpPost("get-history-page")]
    [EnableRateLimiting("MediumCost")]
    [EndpointDescription("Returns the history page.")]
    public async Task<ActionResult<IEnumerable<History>>> GetHistoryPage(
        [FromBody] AdminBox adminBox,
        [FromQuery] int page)
    {
        return await HandleEndpointAsync(() =>
        {
            EnsureValidToken(adminBox.Token, out string username);

            LogAuto(new { Username = username, Page = page },
                toHistory: false); // don't log history into history

            return historyService.GetHistoryEntries(page);
        });
    }
}
