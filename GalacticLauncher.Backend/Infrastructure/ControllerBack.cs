using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace GalacticLauncher.Backend.Infrastructure;

public abstract class ControllerBack(
    ILogger logger
    ) : ControllerBase
{
    protected string IP =>
        HttpContext.Connection.RemoteIpAddress?.ToString()
        ?? "Unknown";

    protected void LogCallToConsole(object? info = null, bool important = false,
        [CallerMemberName] string method = "")
    {
        Action<string?, object?[]> log = important
            ? logger.LogInformation
            : logger.LogDebug;

        Action logAction = info is null
            ? () => log("Address '{IP}' called {Method}()", [IP, method])
            : () => log("Address '{IP}' called {Method}(): {@Info}", [IP, method, info]);

        logAction();
    }

    protected async Task<ActionResult<T>> HandleEndpointAsync<T>(Func<Task<T>> execute,
        [CallerMemberName] string method = "")
    {
        try
        {
            T result = await execute.Invoke();
            return Ok(result);
        }
        catch (ClientFaultException ex)
        {
            logger.LogWarning(
                "Address '{IP}' called {Method}() inproperly: {FaultInfo}",
                IP, method, ex.FaultInfo);

            return Problem(
                detail: ex.Message,
                statusCode: ex.StatusCode
                );
        }
    }

    protected ActionResult<T> HandleEndpoint<T>(Func<T> execute,
        [CallerMemberName] string method = "")
    {
        return HandleEndpointAsync(
            () => Task.FromResult(execute()), method).Result;
    }
}
