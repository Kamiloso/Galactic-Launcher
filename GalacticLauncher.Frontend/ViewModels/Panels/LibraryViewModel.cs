using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.ViewModels.GameButtons;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class LibraryViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _searchGames;

    [ObservableProperty]
    private string? _searchTags;

    public ObservableCollection<GameButtonLibraryViewModel> GameControls { get; } = [];

    public ObservableCollection<Tag> SelectedTags { get; } = [];
    public ObservableCollection<Tag> AvailableTags { get; } = [];

    public enum LibraryViewMode
    {
        YourGames = 0,
        Favorites = 1,
        MoreGames = 2,
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsYourGamesPage))]
    [NotifyPropertyChangedFor(nameof(IsFavoritePage))]
    [NotifyPropertyChangedFor(nameof(IsMoreGamesPage))]
    private LibraryViewMode _currentMode = LibraryViewMode.YourGames;

    public bool IsYourGamesPage => CurrentMode == LibraryViewMode.YourGames;
    public bool IsFavoritePage => CurrentMode == LibraryViewMode.Favorites;
    public bool IsMoreGamesPage => CurrentMode == LibraryViewMode.MoreGames;

    private readonly ICacheRefresher _cacheRefresher;
    private readonly IGameListManager _gameListManager;
    private readonly IGameButtonFactory _gameButtonFactory;
    private readonly ICacheProvider _cacheProvider;

    public LibraryViewModel(
        ICacheRefresher cacheRefresher,
        IGameListManager gameListManager,
        IGameButtonFactory gameButtonFactory,
        ICacheProvider cacheProvider
        )
    {
        _cacheRefresher = cacheRefresher;
        _gameListManager = gameListManager;
        _gameButtonFactory = gameButtonFactory;
        _cacheProvider = cacheProvider;

        _cacheRefresher.OnInitialize += RefreshPage;
        
        _gameListManager.OnLibraryChanged += ReloadGames;

        ReloadTags();
    }

    partial void OnSearchTagsChanged(string? value) => ReloadTags();

    private void ReloadTags()
    {
        AvailableTags.Clear();
        var allTags = _cacheProvider.GetAllTags();

        string search = SearchTags ?? "";

        foreach (var tag in allTags)
        {
            if (SelectedTags.Any(t => t.Id == tag.Id))
                continue;

            if (string.IsNullOrEmpty(search) || tag.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
            {
                AvailableTags.Add(tag);
            }
        }
    }

    [RelayCommand]
    private void SelectTag(Tag tag)
    {
        if (tag == null) return;

        AvailableTags.Remove(tag);
        SelectedTags.Add(tag);

        ReloadGames();
        ReloadTags();
    }

    [RelayCommand]
    private void UnselectTag(Tag tag)
    {
        if (tag == null) return;

        SelectedTags.Remove(tag);

        ReloadTags();
        ReloadGames();
    }

    [RelayCommand]
    public void RefreshPage()
    {
        ReloadGames();
    }

    [RelayCommand]
    public void ChangeViewCommand(LibraryViewMode mode)
    {
        CurrentMode = mode;
    }

    partial void OnCurrentModeChanged(LibraryViewMode value) => ReloadGames();
    partial void OnSearchGamesChanged(string? value) => ReloadGames();

    private void ReloadGames()
    {
        string searchFilter = SearchGames ?? "";

        List<long> gameIdPool = [.. CurrentMode switch
        {
            LibraryViewMode.YourGames => _gameListManager.GetLibraryGames(searchFilter),
            LibraryViewMode.Favorites => _gameListManager.GetFavoriteGames(searchFilter),
            LibraryViewMode.MoreGames => _gameListManager.GetNolibGames(searchFilter),
            _ => throw new NotSupportedException()
        }];

        GameControls.Clear();

        foreach (long id in gameIdPool)
        {
            if (SelectedTags.Any())
            {
                //game has to have all the selected tags to be found during the search
                var gameTags = _cacheProvider.GetGameTags(id);
                bool matchesAllTags = SelectedTags.All(st => gameTags.Any(gt => gt.Id == st.Id));

                if (!matchesAllTags) continue;
            }
            var gbvm = _gameButtonFactory.CreateAndStartLoadingLibrary(id);
            GameControls.Add(gbvm);
        }
    }
}
