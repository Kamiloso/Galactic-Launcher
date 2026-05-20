using System;

namespace GalacticLauncher.Frontend.Domain.Exceptions;

/// <summary>
/// Represents a problem that occurred during an API call to the backend.
/// </summary>
internal class ApiException(string message, int statusCode, Exception? innerException = null)
    : NetworkException(message, statusCode, innerException)
{
    public ApiException(string message, Exception? innerException = null)
        : this(message, 0, innerException) { }
}
