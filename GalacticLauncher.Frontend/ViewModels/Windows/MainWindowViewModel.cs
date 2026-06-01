using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using GalacticLauncher.Frontend.Services.Data;
using System;
using GalacticLauncher.Frontend.ViewModels.Dialogs;

namespace GalacticLauncher.Frontend.ViewModels.Windows;

internal partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SideMenuWidth))]
    private bool _isExpanded = false;

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
    private readonly IDialog _dialog;
    private readonly ICacheRefresher _cacheRefresher;

    public MainWindowViewModel(
        HomeViewModel homeViewModel,
        GameViewModel gameViewModel,
        LibraryViewModel libraryViewModel,
        AdminViewModel adminViewModel,
        INavigator navigator,
        IThemeManager themeManager,
        IDialog dialog,
        ICacheRefresher cacheRefresher,
        INotifications notifications
        )
    {
        _navigator = navigator;
        _homeViewModel = homeViewModel;
        _gameViewModel = gameViewModel;
        _libraryViewModel = libraryViewModel;
        _adminViewModel = adminViewModel;
        _themeManager = themeManager;
        _dialog = dialog;
        _cacheRefresher = cacheRefresher;
        
        _dialog.OnDialogRequested += dvm => CurrentDialog = dvm;
        _cacheRefresher.OnError += notifications.ShowError;

        HandleStartupLoading();

        void HandleStartupLoading()
        {
            LoadingDialogViewModel startupDialog = new(
                "Starting Launcher",
                "Fetching data...");

            _dialog.ShowDialogAndForget(startupDialog);

            _cacheRefresher.OnInitialize +=
                () => _ = startupDialog.Finish();
        }

        _navigator.OnNavigate += InnerNavigate;
        _navigator.NavigateTo<HomeViewModel>();

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
    public void ShowAdmin()
    {
        _navigator.NavigateTo<AdminViewModel>();
    }
}
