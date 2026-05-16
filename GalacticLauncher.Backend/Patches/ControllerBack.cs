using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace GalacticLauncher.Backend.Patches;

public class ControllerBack(
    ILogger logger
    ) : ControllerBase
{
    protected string IP =>
        HttpContext.Connection.RemoteIpAddress?.ToString()
        ?? "Unknown";

    protected void LogCallToConsole(string? moreInfo = null, [CallerMemberName] string caller = "")
    {
        if (moreInfo == null)
        {
            logger.LogInformation("Address '{IP}' called {Method}(...)",
                IP, caller);
        }
        else
        {
            logger.LogInformation("Address '{IP}' called {Method}(...): {MoreInfo}",
                IP, caller, moreInfo);
        }
    }
}
