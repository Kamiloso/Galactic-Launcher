using GalacticLauncher.Frontend.Services.Admin;
using GalacticLauncher.Frontend.ViewModels.Dialogs;
using GalacticLauncher.Frontend.ViewModels.Panels;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

public interface IAdminPanelSelector
{
    Task SelectAdminPanelAsync();
}

internal class AdminPanelSelector(
    IAuthService authService,
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

        bool confirmed = await dialogs.ShowDialogAsync(
            new ConfirmationDialogViewModel(
                "Test dialog",
                "Here you will be telling your username and password."));

        if (!confirmed) return;

        string username = "username";
        string password = "password";

        LoadingDialogViewModel loadingDialog = new(
            "Logging In...",
            "Waiting for the server response...");

        _ = dialogs.ShowDialogAsync(loadingDialog);

        bool authenticated = await authService.TryAuthenticateAsync(username, password);

        await loadingDialog.Finish();

        if (authenticated)
        {
            navigator.NavigateTo<AdminViewModel>();

            await dialogs.ShowDialogAsync(
                new ConfirmationDialogViewModel(
                    "Success!",
                    "This should be OK"));
        }
        else
        {
            await dialogs.ShowDialogAsync(
                new ConfirmationDialogViewModel(
                    "Fail!",
                    "This should be OK"));
        }
    }
}
