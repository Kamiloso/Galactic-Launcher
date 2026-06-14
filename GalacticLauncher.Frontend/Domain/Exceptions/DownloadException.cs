using System;
using System.Runtime.ExceptionServices;

namespace GalacticLauncher.Frontend.Domain.Exceptions;

/// <summary>
/// Represents a problem that occurred during a file download operation.
/// </summary>
internal class DownloadException(string message, int statusCode, Exception? innerException = null)
    : Exception(message, innerException)
{
    public int StatusCode { get; } = statusCode;

    public DownloadException(string message, Exception? innerException = null)
        : this(message, 0, innerException) { }

    public static void WrapThrow(string newMessage, Exception ex)
    {
        if (ex is not DownloadException)
            throw new DownloadException(newMessage, ex);

        ExceptionDispatchInfo.Capture(ex).Throw();
    }
}
