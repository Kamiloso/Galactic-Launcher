using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.ViewModels.Buttons;
using GalacticLauncher.Frontend.ViewModels.Tags;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class LibraryViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _searchGames;

    [ObservableProperty]
    private string? _searchTags;

    public ObservableCollection<GameButtonViewModel> FoundGames { get; } = [];
    public ObservableCollection<TagViewModel> FoundTags { get; } = [];
    public ObservableCollection<TagViewModel> AddedTags { get; } = [];

    public bool HasFoundGames => FoundGames.Count == 0;
    public bool HasNoFoundTags => FoundTags.Count == 0;
    public bool HasAddedTags => AddedTags.Count > 0;
    public bool HasNoTags => !AddedTags.Any() && !FoundTags.Any();

    [ObservableProperty]
    private LibraryViewMode _currentMode = LibraryViewMode.YourGames;

    public bool IsYourGamesPage => CurrentMode == LibraryViewMode.YourGames;
    public bool IsFavoritePage => CurrentMode == LibraryViewMode.Favorites;
    public bool IsMoreGamesPage => CurrentMode == LibraryViewMode.MoreGames;

    public enum LibraryViewMode
    {
        YourGames = 0,
        Favorites = 1,
        MoreGames = 2,
    }

    private List<Tag> _allAvailableTags = [];

    private readonly ICacheRefresher _cacheRefresher;
    private readonly IGameListManager _gameListManager;
    private readonly ICacheProvider _cacheProvider;
    private readonly IGameButtonFactory _gameButtonFactory;

    public LibraryViewModel(
        ICacheRefresher cacheRefresher,
        IGameListManager gameListManager,
        ICacheProvider cacheProvider,
        IGameButtonFactory gameButtonFactory)
    {
        _cacheRefresher = cacheRefresher;
        _gameListManager = gameListManager;
        _cacheProvider = cacheProvider;
        _gameButtonFactory = gameButtonFactory;

        _cacheRefresher.OnRefreshAll += RefreshPage;

        RefreshPage();
    }

    [RelayCommand]
    public void RefreshPage()
    {
        RefreshFoundGames(GetGamePoolIds());
        UpdateTags();
    }

    [RelayCommand]
    public void ChangeView(LibraryViewMode mode)
    {
        CurrentMode = mode;

        OnPropertyChanged(nameof(IsYourGamesPage));
        OnPropertyChanged(nameof(IsFavoritePage));
        OnPropertyChanged(nameof(IsMoreGamesPage));

        RefreshPage();
    }

    partial void OnSearchGamesChanged(string? value)
    {
        ApplyFilters();
    }

    partial void OnSearchTagsChanged(string? value)
    {
        string searchText = (value ?? "").ToLower();

        var activeIds = AddedTags.Select(t => t.Id);
        var potentialTags = _allAvailableTags.Where(t => !activeIds.Contains(t.Id));

        var filtered = string.IsNullOrWhiteSpace(searchText)
            ? potentialTags
            : potentialTags.Where(t => t.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));

        var toRemove = FoundTags.Where(vm => !filtered.Any(t => t.Id == vm.Id)).ToList();
        foreach (var vm in toRemove)
        {
            FoundTags.Remove(vm);
        }

        foreach (var tag in filtered)
        {
            if (!FoundTags.Any(vm => vm.Id == tag.Id))
            {
                FoundTags.Add(new TagViewModel(tag, ToggleTag));
            }
        }
    }

    private void ApplyFilters()
    {
        IEnumerable<long> filteredIds = GetGamePoolIds();

        if (!string.IsNullOrWhiteSpace(SearchGames))
        {
            filteredIds = filteredIds.Where(id =>
                _cacheProvider.GetGameOf(id)?.Name?
                    .Contains(SearchGames, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        if (AddedTags.Any())
        {
            var activeTagIds = AddedTags.Select(t => t.Id).ToList();

            filteredIds = filteredIds.Where(id =>
            {
                var gameTags = _cacheProvider.GetGameTagIds(id);
                return activeTagIds.All(tId => gameTags.Contains(tId));
            });
        }

        RefreshFoundGames(filteredIds);
    }

    private void ToggleTag(TagViewModel tvm)
    {
        if (AddedTags.Remove(tvm))
        {
            tvm.IsActive = false;
            if (!FoundTags.Contains(tvm))
            {
                FoundTags.Add(tvm);
            }
        }
        else
        {
            FoundTags.Remove(tvm);
            tvm.IsActive = true;
            AddedTags.Add(tvm);
        }

        OnPropertyChanged(nameof(HasNoTags));
        OnPropertyChanged(nameof(HasNoFoundTags));
        OnPropertyChanged(nameof(HasAddedTags));

        ApplyFilters();
    }

    private void UpdateTags()
    {
        _allAvailableTags = [.. _cacheProvider.GetAllTags()];

        RefreshTagsList(FoundTags);
        RefreshTagsList(AddedTags);
    }

    private void RefreshTagsList(ObservableCollection<TagViewModel> targetCollection)
    {
        var currentAvailableTags = _cacheProvider.GetAllTags().ToDictionary(t => t.Id);

        var toRemove = targetCollection.Where(vm => !currentAvailableTags.ContainsKey(vm.Id)).ToList();
        foreach (var vm in toRemove)
        {
            targetCollection.Remove(vm);
        }

        foreach (var id in currentAvailableTags.Keys)
        {
            var tagData = currentAvailableTags[id];
            var existingVm = targetCollection.FirstOrDefault(vm => vm.Id == id);

            if (existingVm != null)
            {
                if (existingVm.Name != tagData.Name) existingVm.Name = tagData.Name;
                if (existingVm.Description != tagData.Description) existingVm.Description = tagData.Description;
            }
            else
            {
                targetCollection.Add(new TagViewModel(tagData, ToggleTag));
            }
        }
    }

    private IEnumerable<long> GetGamePoolIds(Predicate<long>? predicate = null)
    {
        var collection =  CurrentMode switch
        {
            LibraryViewMode.YourGames => _gameListManager.GetLibraryGames(),
            LibraryViewMode.Favorites => _gameListManager.GetFavoriteGames(),
            LibraryViewMode.MoreGames => _gameListManager.GetAllGames(),
            _ => throw new NotSupportedException()
        };

        return collection.Where(id => predicate?.Invoke(id) ?? true);
    }

    private void RefreshFoundGames(IEnumerable<long> gamePoolIds)
    {
        FoundGames.Clear();

        foreach (long id in gamePoolIds)
        {
            var gbvm = _gameButtonFactory.CreateAndStartLoading(id);
            FoundGames.Add(gbvm);
        }

        OnPropertyChanged(nameof(HasFoundGames));
        OnPropertyChanged(nameof(HasNoFoundTags));
        OnPropertyChanged(nameof(HasAddedTags));
        OnPropertyChanged(nameof(HasNoTags));
    }
}
