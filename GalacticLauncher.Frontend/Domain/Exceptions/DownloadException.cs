using System;

namespace GalacticLauncher.Frontend.Domain.Exceptions;

/// <summary>
/// Represents a problem that occurred during a file download operation.
/// </summary>
internal class DownloadException(string message, int statusCode, Exception? innerException = null)
    : NetworkException(message, statusCode, innerException)
{
    public DownloadException(string message, Exception? innerException = null)
        : this(message, 0, innerException) { }
}
