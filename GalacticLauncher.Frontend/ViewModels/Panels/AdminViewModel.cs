using CommunityToolkit.Mvvm.ComponentModel;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.ViewModels.AdminPanels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using System;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal class AdminViewModel : ObservableObject
{
    // TODO: Magda
    // Make this thing more compact. Use MVVM toolkit,
    // to reduce boilerplate, and make it more readable.

    // Also rename CurrentPage and CurrentActivePage,
    // because for now they look really similar,
    // but are actually different things.
    // Maybe CurrentPage and CurrentPageName?

    // Also fix this in the MainWindowViewModel, since it has the same issue.

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

    private readonly AdGamesViewModel _gamesViewModel;
    private readonly AdTagsViewModel _tagsViewModel;
    private readonly AdUsersViewModel _usersViewModel;
    private readonly INavigator _navigator;

    public AdminViewModel(
        AdGamesViewModel gamesViewModel,
        AdTagsViewModel tagsViewModel,
        AdUsersViewModel usersViewModel,
        INavigator navigator
        )
    {
        _gamesViewModel = gamesViewModel;
        _tagsViewModel = tagsViewModel;
        _usersViewModel = usersViewModel;
        _navigator = navigator;

        _navigator.OnAdminPanelNavigate += InnerNavigate;
        _navigator.NavigateTo<AdGamesViewModel>();

        void InnerNavigate(Type pageType, object[] args)
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
                nav.OnActivate(args);
            }
        }
    }

    public void ShowGames() =>
        _navigator.AdminPanelNavigateTo<AdGamesViewModel>();

    public void ShowTags() =>
        _navigator.AdminPanelNavigateTo<AdTagsViewModel>();

    public void ShowUsers() =>
        _navigator.AdminPanelNavigateTo<AdUsersViewModel>();
}

