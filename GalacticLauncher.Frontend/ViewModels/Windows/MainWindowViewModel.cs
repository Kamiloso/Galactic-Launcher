using System;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.ViewModels.Panels;

namespace GalacticLauncher.Frontend.ViewModels.Windows;

internal partial class MainWindowViewModel : NotifierBase, INotifyPropertyChanged
{
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

    private readonly Navigator _navigator;
    private readonly HomeViewModel _homeViewModel;
    private readonly GameViewModel _gameViewModel;
    private readonly LibraryViewModel _libraryViewModel;
    private readonly AdminViewModel _adminViewModel;
    private readonly ThemeManager _themeManager;

    public MainWindowViewModel(
        Navigator navigator,
        HomeViewModel homeViewModel,
        GameViewModel gameViewModel,
        LibraryViewModel libraryViewModel,
        AdminViewModel adminViewModel,
        ThemeManager themeManager
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

        CurrentPage = homeViewModel;

        _navigator.OnNavigate += SetMainPage;

        void SetMainPage(Type pageType) // place it here to not accidentally use it outside
        {
            CurrentPage = pageType switch
            {
                _ when pageType == typeof(HomeViewModel) => _homeViewModel,
                _ when pageType == typeof(GameViewModel) => _gameViewModel,
                _ when pageType == typeof(LibraryViewModel) => _libraryViewModel,
                _ when pageType == typeof(AdminViewModel) => _adminViewModel,
                _ => throw new ArgumentException($"Unknown page type: {pageType.FullName}")
            };

            if (CurrentPage is INavigationAware nav)
            {
                nav.OnActivate();
            }
        }
    }
    [RelayCommand]
    public void ShowHome() => _navigator.NavigateTo<HomeViewModel>();
    [RelayCommand]
    public void ShowGame() => _navigator.NavigateTo<GameViewModel>();
    [RelayCommand]
    public void ShowLibrary() => _navigator.NavigateTo<LibraryViewModel>();
    [RelayCommand]
    public void ShowAdmin() => _navigator.NavigateTo<AdminViewModel>();
    public ICommand SwitchThemeCommand { get; }

    private void OnThemeErrorOccured(string message)
    {

    }
}
