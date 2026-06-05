using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.Services.Admin;
using GalacticLauncher.Frontend.ViewModels.Panels;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

public interface IAdminPanelSelector
{
    Task SelectAdminPanelAsync();
}

internal class AdminPanelSelector(
    IAuthService authService,
    IPreferenceManager preferenceManager,
    IDialogs dialogs,
    INavigator navigator) : IAdminPanelSelector
{
    public async Task SelectAdminPanelAsync()
    {
        if (authService.IsValidSession)
        {
            navigator.NavigateTo<AdminViewModel>();
            return;
        }

        var credentials = await dialogs.ShowLoginDialogAsync(
            "Login",
            "Please enter your credentials to access the admin panel.",
            username: preferenceManager.LastUsername);

        if (credentials == null) return;

        string username = credentials.Value.Username;
        string password = credentials.Value.Password;

        preferenceManager.LastUsername = username;

        bool authenticated = await dialogs.ShowLoadingDialogAsync(
            "Logging In...",
            "Waiting for the server response...",
            authService.TryAuthenticateAsync(username, password));

        if (authenticated)
        {
            navigator.NavigateTo<AdminViewModel>();
        }
        else
        {
            bool tryAgain = await dialogs.ShowConfirmationDialogAsync(
                "Authentication Failed",
                "Failed to obtain the session token. Do you want to try again?",
                textYes: "Retry", textNo: "Cancel");

            if (tryAgain)
            {
                // I know, I know it may cause a stack overflow,
                // at least in theory, but we just ignore it for simplicity.

                await SelectAdminPanelAsync();
            }
        }
    }
}
