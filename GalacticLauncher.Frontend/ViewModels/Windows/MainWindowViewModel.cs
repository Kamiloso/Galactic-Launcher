using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using System;
using System.Threading.Tasks;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Admin;
using GalacticLauncher.Core;
using GalacticLauncher.Frontend.Services.Data;

namespace GalacticLauncher.Frontend.ViewModels.Windows;

internal partial class MainWindowViewModel : ObservableObject
{
    private const string ADMIN_TITLE = "ADMIN";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SideMenuWidth))]
    private bool _isExpanded;

    [ObservableProperty]
    private bool _isAdminVisible;

    [ObservableProperty]
    private string _adminTitleText = ADMIN_TITLE;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHomePage))]
    [NotifyPropertyChangedFor(nameof(IsLibraryPage))]
    [NotifyPropertyChangedFor(nameof(IsAdminPage))]
    [NotifyPropertyChangedFor(nameof(IsGamePage))]
    private object? _currentPage;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDialogVisible))]
    private object? _currentDialog;

    public double SideMenuWidth => IsExpanded ? 200 : 84;
    public bool IsDialogVisible => CurrentDialog != null;

    public bool IsHomePage => CurrentPage is HomeViewModel;
    public bool IsLibraryPage => CurrentPage is LibraryViewModel;
    public bool IsAdminPage => CurrentPage is AdminViewModel;
    public bool IsGamePage => CurrentPage is GameViewModel;

    private readonly HomeViewModel _homeViewModel;
    private readonly GameViewModel _gameViewModel;
    private readonly LibraryViewModel _libraryViewModel;
    private readonly AdminViewModel _adminViewModel;
    private readonly INavigator _navigator;
    private readonly IThemeManager _themeManager;
    private readonly ICacheRefresher _cacheRefresher;
    private readonly IAuthService _authService;
    private readonly IPreferenceManager _preferenceManager;
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
        IAuthService authService,
        IPreferenceManager preferenceManager,
        IAdminPanelSelector adminPanelSelector,
        IDialogs dialog)
    {
        _navigator = navigator;
        _homeViewModel = homeViewModel;
        _gameViewModel = gameViewModel;
        _libraryViewModel = libraryViewModel;
        _adminViewModel = adminViewModel;
        _themeManager = themeManager;
        _cacheRefresher = cacheRefresher;
        _authService = authService;
        _preferenceManager = preferenceManager;
        _adminPanelSelector = adminPanelSelector;
        _dialogs = dialog;

        IsExpanded = preferenceManager.IsMenuExpanded;
        IsAdminVisible = preferenceManager.IsAdminPanelVisible;

        ConfigureNavigation();
        ConfigureAdminTitle();
        ConfigureLoadingDialog();
    }

    private void ConfigureNavigation()
    {
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

    private void ConfigureAdminTitle()
    {
        _ = SpinInfinitely();

        async Task SpinInfinitely()
        {
            while (true)
            {
                TimeSpan toExpire = _authService.TimeToExpiration();
                bool isValidSession = _authService.IsValidSession;

                AdminTitleText = isValidSession
                    ? $"{ADMIN_TITLE} {Utils.FormatTimeSpan(toExpire)}"
                    : $"{ADMIN_TITLE}";

                await Task.Delay(50);
            }
        }
    }

    private void ConfigureLoadingDialog()
    {
        _dialogs.OnDialogChanged += dvm => CurrentDialog = dvm;

        Func<Task> close = _dialogs.ShowLoadingDialog(
            "Starting Launcher",
            "Fetching data...",
            fakeLoadingTime: 1000);

        _cacheRefresher.OnInitialize +=
            async () => await close();
    }

    partial void OnIsExpandedChanged(bool value)
    {
        _preferenceManager.IsMenuExpanded = value;
    }

    partial void OnIsAdminVisibleChanged(bool value)
    {
        _preferenceManager.IsAdminPanelVisible = value;
    }

    [RelayCommand]
    public void ToggleMenu()
    {
        IsExpanded = !IsExpanded;
    }

    [RelayCommand]
    public void ToggleAdminVisible()
    {
        IsAdminVisible = !IsAdminVisible;
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
