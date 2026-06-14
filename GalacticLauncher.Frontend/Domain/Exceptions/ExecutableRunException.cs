using System;
using System.Runtime.ExceptionServices;

namespace GalacticLauncher.Frontend.Domain.Exceptions;

/// <summary>
/// Represents a problem that occurred while trying to run an executable.
/// </summary>
internal class ExecutableRunException(string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public static void WrapThrow(string newMessage, Exception ex)
    {
        if (ex is not ExecutableRunException)
            throw new ExecutableRunException(newMessage, ex);

        ExceptionDispatchInfo.Capture(ex).Throw();
    }
}
