using System;

namespace GalacticLauncher.Frontend.Domain.Exceptions;

/// <summary>
/// Base exception for custom network-related exceptions (ApiException, DownloadException, etc.)
/// </summary>
internal abstract class NetworkException(string message, int statusCode, Exception? innerException = null)
    : Exception(message, innerException)
{
    public int StatusCode { get; } = statusCode;

    public NetworkException(string message, Exception? innerException = null)
        : this(message, 0, innerException) { }
}
