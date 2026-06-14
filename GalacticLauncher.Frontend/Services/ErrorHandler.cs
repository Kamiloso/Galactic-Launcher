#pragma warning disable CS0067 // for unused events
using System;

namespace GalacticLauncher.Frontend.Services;

public interface IErrorHandler
{
    event Action<string, string>? OnInfo;
    event Action<string, string>? OnWarning;
    event Action<string, string>? OnError;
    event Action<string, string>? OnSuccess;

    void HandleApiError(int code, bool showNoInternet = false);
}

public class ErrorHandler : IErrorHandler
{
    public event Action<string, string>? OnInfo;
    public event Action<string, string>? OnWarning;
    public event Action<string, string>? OnError;
    public event Action<string, string>? OnSuccess;

    public void HandleApiError(int code, bool showNoInternet = false)
    {
        if (code == 0 && showNoInternet)
            OnWarning?.Invoke("Offline Mode", "Failed to reach the server.");

        if (code / 100 == 4)
            OnError?.Invoke("Client Error", $"An error occurred on the client side: HTTP {code}");

        if (code / 100 == 5)
            OnError?.Invoke("Server Error", $"An error occurred on the server side: HTTP {code}");
    }
}
