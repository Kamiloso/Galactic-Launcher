using GalacticLauncher.Backend.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GalacticLauncher.Backend.Infrastructure;

public partial class ControllerBack
{
    protected async Task<ActionResult<T>> HandleEndpointAsync<T>(Func<Task<T>> execute)
    {
        try
        {
            T result = await execute.Invoke();
            return Ok(result);
        }
        catch (ClientFaultException ex)
        {
            try
            {
                LogAuto(ex.FaultInfo,
                    verb: "faulted during",
                    importance: LogLevel.Warning);
            }
            catch { }

            return Problem(
                detail: ex.Message,
                statusCode: ex.StatusCode);
        }
        catch (Exception ex)
        {
            try
            {
                LogAuto(ex.ToString(),
                    verb: "crashed the query during",
                    importance: LogLevel.Error,
                    suppressConsole: true); // it is logged anyway
            }
            catch { }

            throw;
        }
    }

    protected ActionResult<T> HandleEndpoint<T>(Func<T> execute)
    {
        return HandleEndpointAsync(
            () => Task.FromResult(execute())).Result;
    }

    protected async Task<ActionResult> HandleEndpointAsync(Func<Task> execute)
    {
        ActionResult<int> response = await HandleEndpointAsync(
            async () =>
            {
                await execute.Invoke();
                return 0;
            });

        return response.Result is OkObjectResult
            ? Ok()
            : response.Result!;
    }

    protected ActionResult HandleEndpoint(Action execute)
    {
        ActionResult<int> response = HandleEndpoint(
            () =>
            {
                execute.Invoke();
                return 0;
            });

        return response.Result is OkObjectResult
            ? Ok()
            : response.Result!;
    }
}
