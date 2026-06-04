using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.Services.Handlers;

public interface IErrorHandler
{
    void HandleApiError(int code, bool showNoInternet = false);
}

internal class ErrorHandler(INotifications notifications) : IErrorHandler
{
    public void HandleApiError(int code, bool showNoInternet = false)
    {
        if (code == 0 && showNoInternet)
            notifications.ShowWarning("Offline Mode", "Could not reach the server.");

        if (code / 100 == 4)
            notifications.ShowError("Client Error", $"An error occurred on the client side (Code: {code}).");

        if (code / 100 == 5)
            notifications.ShowError("Server Error", $"An error occurred on the server side (Code: {code}).");
    }
}
