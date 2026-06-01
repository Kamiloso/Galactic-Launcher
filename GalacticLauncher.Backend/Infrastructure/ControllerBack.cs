using GalacticLauncher.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace GalacticLauncher.Backend.Infrastructure;

public abstract partial class ControllerBack(
    ILogger logger,
    IHistoryService historyService) : ControllerBase
{
    protected string IP =>
        HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
}
