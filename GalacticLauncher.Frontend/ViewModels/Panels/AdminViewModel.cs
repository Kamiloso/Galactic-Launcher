using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.ViewModels.AdminPanels;
using System;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal class AdminViewModel : NotifierBase
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
    private readonly AdGamesViewModel _gamesViewModel;
    private readonly AdTagsViewModel _tagsViewModel;
    private readonly AdUsersViewModel _usersViewModel;

    public AdminViewModel(
        Navigator navigator,
        AdGamesViewModel gamesViewModel,
        AdTagsViewModel tagsViewModel,
        AdUsersViewModel usersViewModel
        )
    {
        _navigator = navigator;
        _gamesViewModel = gamesViewModel;
        _tagsViewModel = tagsViewModel;
        _usersViewModel = usersViewModel;

        SetAdminPage(typeof(AdTagsViewModel));

        void SetAdminPage(Type pageType)
        {
            CurrentPage = pageType switch
            {
                _ when pageType == typeof(AdGamesViewModel) => _gamesViewModel,
                _ when pageType == typeof(AdTagsViewModel) => _tagsViewModel,
                _ when pageType == typeof(AdUsersViewModel) => _usersViewModel,
                _ => throw new ArgumentException($"Unknown page type: {pageType.FullName}")
            };

            if (CurrentPage is INavigationAware nav)
            {
                nav.OnActivate();
            }
        }
    }

    public void ShowGames() => _navigator.AdminPanelNavigateTo<AdGamesViewModel>();
    public void ShowTags() => _navigator.AdminPanelNavigateTo<AdTagsViewModel>();
    public void ShowUsers() => _navigator.AdminPanelNavigateTo<AdUsersViewModel>();
}

