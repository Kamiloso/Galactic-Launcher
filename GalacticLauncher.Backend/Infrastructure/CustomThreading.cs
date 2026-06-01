namespace GalacticLauncher.Backend.Infrastructure;

public static class CustomThreading
{
    public static async Task LogTaskErrors(Task task, string message, ILogger logger)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            logger.LogError("{Message}: {More}", message, ex.ToString());
        }
    }
}
