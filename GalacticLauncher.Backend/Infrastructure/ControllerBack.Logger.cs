#pragma warning disable CA2254 // for simplicity
using GalacticLauncher.Core;

namespace GalacticLauncher.Backend.Infrastructure;

public partial class ControllerBack
{
    protected void LogAuto(object? source = null,
        string verb = "called",
        LogLevel importance = LogLevel.Debug,
        bool toHistory = false,
        bool suppressConsole = false,
        long? idGame = null)
    {
        string message = source is null
            ? $"Address {{IP}} {verb} {{Endpoint}}"
            : $"Address {{IP}} {verb} {{Endpoint}} with {{Source}}";

        object?[] args = source is null
            ? [IP, Endpoint]
            : [IP, Endpoint, source];

        LogManual(message, args,
            importance: importance,
            toHistory: toHistory,
            suppressConsole: suppressConsole,
            idGame: idGame);
    }

    protected void LogManual(string message, object?[] args,
        LogLevel importance = LogLevel.Information,
        bool suppressConsole = false,
        bool toHistory = false,
        long? idGame = null)
    {
        string info = TextUtils.FormatString(message, args);

        if (!suppressConsole)
        {
            logger.Log(importance, info);
        }

        if (toHistory)
        {
            _ = CustomThreading.LogTaskErrors(
                historyService.LogToHistory(info, idGame),
                "Error logging history to database",
                logger);
        }
    }
}
