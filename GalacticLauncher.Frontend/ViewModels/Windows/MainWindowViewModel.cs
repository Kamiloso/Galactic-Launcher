using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using GalacticLauncher.Frontend.Services.Data;
using System;
using System.Threading.Tasks;
using GalacticLauncher.Frontend.Services;

namespace GalacticLauncher.Frontend.ViewModels.Windows;

internal partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SideMenuWidth))]
    private bool _isExpanded = true;

    public double SideMenuWidth => IsExpanded ? 200 : 84;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHomePage))]
    [NotifyPropertyChangedFor(nameof(IsLibraryPage))]
    [NotifyPropertyChangedFor(nameof(IsAdminPage))]
    [NotifyPropertyChangedFor(nameof(IsGamePage))]
    public object? _currentPage;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDialogVisible))]
    private object? _currentDialog;

    public bool IsHomePage => CurrentPage is HomeViewModel;
    public bool IsLibraryPage => CurrentPage is LibraryViewModel;
    public bool IsAdminPage => CurrentPage is AdminViewModel;
    public bool IsGamePage => CurrentPage is GameViewModel;
    public bool IsDialogVisible => CurrentDialog != null;

    private readonly HomeViewModel _homeViewModel;
    private readonly GameViewModel _gameViewModel;
    private readonly LibraryViewModel _libraryViewModel;
    private readonly AdminViewModel _adminViewModel;
    private readonly INavigator _navigator;
    private readonly IThemeManager _themeManager;
    private readonly ICacheRefresher _cacheRefresher;
    private readonly IAdminPanelSelector _adminPanelSelector;
    private readonly IDialogs _dialogs;

    public MainWindowViewModel(
        HomeViewModel homeViewModel,
        GameViewModel gameViewModel,
        LibraryViewModel libraryViewModel,
        AdminViewModel adminViewModel,
        INavigator navigator,
        IThemeManager themeManager,
        ICacheRefresher cacheRefresher,
        IAdminPanelSelector adminPanelSelector,
        IErrorHandler errorHandler,
        INotifications notifications,
        IDialogs dialog)
    {
        _navigator = navigator;
        _homeViewModel = homeViewModel;
        _gameViewModel = gameViewModel;
        _libraryViewModel = libraryViewModel;
        _adminViewModel = adminViewModel;
        _themeManager = themeManager;
        _cacheRefresher = cacheRefresher;
        _adminPanelSelector = adminPanelSelector;
        _dialogs = dialog;

        // Error handling

        errorHandler.OnInfo += notifications.ShowInfo;
        errorHandler.OnWarning += notifications.ShowWarning;
        errorHandler.OnError += notifications.ShowError;
        errorHandler.OnSuccess += notifications.ShowSuccess;

        // Navigation

        _navigator.OnNavigate += InnerNavigate;
        _navigator.NavigateTo<HomeViewModel>();

        // Loading dialog

        _dialogs.OnDialogChanged += dvm => CurrentDialog = dvm;

        Func<Task> finish = _dialogs.ShowLoadingDialogAsync(
            "Starting Launcher",
            "Fetching data...");

        _cacheRefresher.OnInitialize += () => finish();

        // Keep it local to not accidentally call it from somewhere else

        void InnerNavigate(Type pageType, object[] args)
        {
            CurrentPage = pageType switch
            {
                _ when pageType == typeof(HomeViewModel) => _homeViewModel,
                _ when pageType == typeof(LibraryViewModel) => _libraryViewModel,
                _ when pageType == typeof(AdminViewModel) => _adminViewModel,
                _ when pageType == typeof(GameViewModel) => _gameViewModel,
                _ => throw new NotSupportedException()
            };

            if (CurrentPage is INavigationAware nav)
            {
                nav.OnActivate(args);
            }
        }
    }

    [RelayCommand]
    public void ToggleMenu()
    {
        IsExpanded = !IsExpanded;
    }

    [RelayCommand]
    public void SwitchTheme()
    {
        _themeManager.ToggleTheme();
    }

    [RelayCommand]
    public void ShowHome()
    {
        _navigator.NavigateTo<HomeViewModel>();
    }

    [RelayCommand]
    public void ShowLibrary()
    {
        _navigator.NavigateTo<LibraryViewModel>();
    }

    [RelayCommand]
    public async Task ShowAdmin()
    {
        await _adminPanelSelector.SelectAdminPanelAsync();
    }
}
