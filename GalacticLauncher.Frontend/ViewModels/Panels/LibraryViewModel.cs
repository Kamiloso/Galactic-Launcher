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

    private readonly Dictionary<long, GameButtonLibraryViewModel> _buttonStore = [];

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
        _gameListManager.OnListsChanged += RefreshPage;

        SelectedTags.CollectionChanged +=
            (_, _) => RefreshPage();

        RefreshPage();
    }

    [RelayCommand]
    public void RefreshPage()
    {
        ReloadGames();
        ReloadTags();
    }

    [RelayCommand]
    public void ChangeView(LibraryViewMode mode)
    {
        CurrentMode = mode;
    }

    [RelayCommand]
    private void SelectTag(Tag tag)
    {
        SelectedTags.Add(tag);
    }

    [RelayCommand]
    private void UnselectTag(Tag tag)
    {
        SelectedTags.Remove(tag);
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

        List<GameButtonLibraryViewModel> targetButtonList = [];

        foreach (long id in gameIdPool)
        {
            List<Tag> gameTags = [.. _cacheProvider.GetTagsByGameId(id)];

            if (!SelectedTags.Any() ||
                SelectedTags.Any(t1 => gameTags.Any(t2 => t2.Id == t1.Id)))
            {
                if (!_buttonStore.TryGetValue(id, out var gbvm))
                {
                    gbvm = _gameButtonFactory.CreateAndStartLoadingLibrary(id);
                    _buttonStore.Add(id, gbvm);
                }

                targetButtonList.Add(gbvm);
            }
        }

        GameControls.Clear();

        foreach (var gbvm in targetButtonList)
        {
            GameControls.Add(gbvm);
        }
    }

    partial void OnSearchTagsChanged(string? value) => ReloadTags();

    private void ReloadTags()
    {
        string searchFilter = SearchTags ?? "";

        AvailableTags.Clear();

        List<Tag> allTags = [..
            _cacheProvider.GetAllTags()];

        List<Tag> searchTags = [.. allTags
            .Where(t1 => !SelectedTags.Any(t2 => t1.Id == t2.Id))];

        foreach (Tag tag in searchTags)
        {
            bool isFiltering = searchFilter != "";
            bool searched = tag.Name.Contains(searchFilter, StringComparison.OrdinalIgnoreCase);

            if (!isFiltering || searched)
            {
                AvailableTags.Add(tag);
            }
        }
    }
}
