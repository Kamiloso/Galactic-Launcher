using System;

namespace GalacticLauncher.Frontend.Services.Networking;

internal class ApiException(string message, int statusCode, Exception? innerException = null)
    : Exception(message, innerException)
{
    public int StatusCode { get; } = statusCode;

    public ApiException(string message, Exception? innerException = null)
        : this(message, 0, innerException) { }
}
