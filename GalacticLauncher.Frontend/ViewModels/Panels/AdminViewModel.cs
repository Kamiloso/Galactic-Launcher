using CommunityToolkit.Mvvm.ComponentModel;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.ViewModels.AdminPanels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using System;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class AdminViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsGamesPage))]
    [NotifyPropertyChangedFor(nameof(IsTagsPage))]
    [NotifyPropertyChangedFor(nameof(IsUsersPage))]
    private object? _currentPage;

    public bool IsGamesPage => CurrentPage is AdGamesViewModel;
    public bool IsTagsPage => CurrentPage is AdTagsViewModel;
    public bool IsUsersPage => CurrentPage is AdUsersViewModel;

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
                _ => throw new NotSupportedException()
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
