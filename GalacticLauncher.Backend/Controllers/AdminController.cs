using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Backend.Services;
using GalacticLauncher.Core.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using GalacticLauncher.Backend.Domain.Exceptions;

namespace GalacticLauncher.Backend.Controllers;

[ApiController]
[Route("admin")]
public class AdminController(
    ILogger<TestingController> logger,
    IAdminService adminService,
    IDataUpdateService dataUpdateService) : ControllerBack(logger)
{
    [HttpPost("req-admin")]
    [EnableRateLimiting("ReqCost")]
    [EndpointDescription("Asks the server for the admin token.")]
    public ActionResult<LoginResult> ReqAdmin(
        [FromBody] LoginRequest loginRequest)
    {
        LogCallToConsole(new { username = loginRequest.Username }, important: true);

        return HandleEndpoint(() =>
            adminService.AuthenticateAdmin(loginRequest));
    }

    [HttpPost("update-game")]
    [EnableRateLimiting("HighCost")]
    [EndpointDescription("Updates the game data on the server. Requires admin token.")]
    public async Task<ActionResult> UpdateGameData(
        [FromBody] GameDataUpdate gameDataUpdate)
    {
        var (token, gameData) = gameDataUpdate;

        bool validated = adminService.TryValidateSession(token, out string username);

        LogCallToConsole(validated
            ? new { username, gameData }
            : new { token }
            , important: true);

        return await HandleEndpointAsync(() =>
        {
            if (!validated)
                throw ClientFaultException.Unauthorized401("Invalid or expired admin token.");

            return dataUpdateService.UpdateGameData(gameData);
        });
    }
}
