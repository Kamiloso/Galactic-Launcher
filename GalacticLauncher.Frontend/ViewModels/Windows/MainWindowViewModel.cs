using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using System;
using System.Windows.Input;

namespace GalacticLauncher.Frontend.ViewModels.Windows;

internal partial class MainWindowViewModel : ObservableObject
{
    private const double NARROW_MENU = 84;
    private const double EXPANDED_MENU = 200;

    [ObservableProperty]
    private double _sideMenuWidth = NARROW_MENU;

    [ObservableProperty]
    private bool _isExpanded = false;

    [RelayCommand]
    public void ToggleMenu()
    {
        SideMenuWidth = SideMenuWidth == NARROW_MENU
            ? EXPANDED_MENU
            : NARROW_MENU;
        IsExpanded = !IsExpanded;
    }

    public ICommand SwitchThemeCommand { get; }

    private object? _currentPage;
    public object? CurrentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentActivePage));
        }
    }
    public string CurrentActivePage => CurrentPage?.GetType().Name ?? "";

    private readonly HomeViewModel _homeViewModel;
    private readonly GameViewModel _gameViewModel;
    private readonly LibraryViewModel _libraryViewModel;
    private readonly AdminViewModel _adminViewModel;
    private readonly INavigator _navigator;
    private readonly IThemeManager _themeManager;

    public MainWindowViewModel(
        HomeViewModel homeViewModel,
        GameViewModel gameViewModel,
        LibraryViewModel libraryViewModel,
        AdminViewModel adminViewModel,
        INavigator navigator,
        IThemeManager themeManager
        )
    {
        _navigator = navigator;
        _homeViewModel = homeViewModel;
        _gameViewModel = gameViewModel;
        _libraryViewModel = libraryViewModel;
        _adminViewModel = adminViewModel;
        _themeManager = themeManager;

        SwitchThemeCommand = new RelayCommand(() => _themeManager.ToggleTheme());
        _themeManager.ThemeErrorOccurred += OnThemeErrorOccured;

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
                _ => throw new ArgumentException($"Unknown page type: {pageType.FullName}")
            };

            if (CurrentPage is INavigationAware nav)
            {
                nav.OnActivate(args);
            }
        }
    }

    [RelayCommand]
    public void ShowHome() =>
        _navigator.NavigateTo<HomeViewModel>();

    [RelayCommand]
    public void ShowLibrary() =>
        _navigator.NavigateTo<LibraryViewModel>();

    [RelayCommand]
    public void ShowAdmin() =>
        _navigator.NavigateTo<AdminViewModel>();

    private void OnThemeErrorOccured(string message)
    {

    }
}
