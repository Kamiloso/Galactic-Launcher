using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.Services.Admin;
using GalacticLauncher.Frontend.ViewModels.Panels;
using System;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

public interface IAdminPanelSelector
{
    Task SelectAdminPanelAsync();
    Task RefreshAdminPanelAsync();
}

internal class AdminPanelSelector : IAdminPanelSelector
{
    private string _lastUsername = "";
    private string _lastPassword = "";

    private readonly IAuthService _authService;
    private readonly IPreferenceManager _preferenceManager;
    private readonly IDialogs _dialogs;
    private readonly INotifications _notifications;
    private readonly INavigator _navigator;

    public AdminPanelSelector(
        IAuthService authService,
        IPreferenceManager preferenceManager,
        IDialogs dialogs,
        INotifications notifications,
        INavigator navigator)
    {
        _authService = authService;
        _preferenceManager = preferenceManager;
        _dialogs = dialogs;
        _notifications = notifications;
        _navigator = navigator;

        _ = SpinInfinitely();
    }

    private async Task SpinInfinitely()
    {
        while (true)
        {
            if (!_authService.IsValidSession &&
                _navigator.PageType == typeof(AdminViewModel))
            {
                _navigator.NavigateTo<HomeViewModel>();

                _notifications.ShowWarning(
                    "Session Expired",
                    "Your admin session has expired.");
            }

            await Task.Delay(50);
        }
    }

    public async Task SelectAdminPanelAsync()
    {
        if (_authService.IsValidSession)
        {
            _navigator.NavigateTo<AdminViewModel>();
        }
        else
        {
            await TryLoginAndThen(() => _navigator.NavigateTo<AdminViewModel>(),
                askForCredentials: true);
        }
    }

    public async Task RefreshAdminPanelAsync()
    {
        bool confirmed = await _dialogs.ShowConfirmationDialogAsync(
            "Refresh Session",
            "Do you want to obtain a new session token from the server?");

        if (confirmed)
        {
            await TryLoginAndThen(() => _navigator.NavigateTo<AdminViewModel>(),
                askForCredentials: false);
        }
    }

    private async Task TryLoginAndThen(Action onValidate, bool askForCredentials)
    {
        string username, password;

        if (askForCredentials)
        {
            var credentials = await _dialogs.ShowLoginDialogAsync(
                "Login",
                "Please enter your credentials to access the admin panel.",
                username: _preferenceManager.LastUsername);

            if (credentials == null) return;

            username = credentials.Value.Username;
            password = credentials.Value.Password;

            _preferenceManager.LastUsername = username;
        }
        else
        {
            username = _lastUsername;
            password = _lastPassword;
        }

        bool authenticated = await _dialogs.ShowLoadingDialogAsync(
            "Logging In...",
            "Waiting for the server response...",
            _authService.TryAuthenticateAsync(username, password),
            fakeLoadingTime: 300);

        if (authenticated)
        {
            _lastUsername = username;
            _lastPassword = password;

            onValidate();
        }
        else
        {
            bool tryAgain = await _dialogs.ShowConfirmationDialogAsync(
                "Authentication Failed",
                "Failed to obtain the session token. Do you want to try again?",
                textYes: "Retry", textNo: "Cancel");

            if (tryAgain)
            {
                await TryLoginAndThen(onValidate,
                    askForCredentials: true);
            }
        }
    }
}
