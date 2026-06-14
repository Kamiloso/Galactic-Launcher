using System;
using System.Runtime.ExceptionServices;

namespace GalacticLauncher.Frontend.Domain.Exceptions;

/// <summary>
/// Represents a problem that occurred during an API call to the backend.
/// </summary>
internal class ApiException(string message, int statusCode, Exception? innerException = null)
    : Exception(message, innerException)
{
    public int StatusCode { get; } = statusCode;

    public ApiException(string message, Exception? innerException = null)
        : this(message, 0, innerException) { }

    public static void WrapThrow(string newMessage, Exception ex)
    {
        if (ex is not ApiException)
            throw new ApiException(newMessage, ex);

        ExceptionDispatchInfo.Capture(ex).Throw();
    }
}
