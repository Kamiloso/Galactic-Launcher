using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Core.Dto;
using GalacticLauncher.Backend.Services;

namespace GalacticLauncher.Backend.Controllers;

[ApiController]
[Route("admin")]
public class AdminController(
    ILogger<TestingController> logger,
    IAdminService adminService
    ) : ControllerBack(logger)
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
}