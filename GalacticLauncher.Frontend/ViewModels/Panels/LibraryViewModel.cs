using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.Services.Images;
using GalacticLauncher.Frontend.ViewModels.Buttons;
using GalacticLauncher.Frontend.ViewModels.Tags;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class LibraryViewModel : ObservableObject
{
    public enum LibraryViewMode
    {
        YourGames = 0,
        Favorites = 1,
        MoreGames = 2,
    }

    [ObservableProperty]
    private LibraryViewMode _currentMode = 0;

    [ObservableProperty]
    private string? _searchGames;

    [ObservableProperty]
    private string? _searchTags;

    public ObservableCollection<GameButtonViewModel> FoundGames { get; } = [];
    public ObservableCollection<TagViewModel> FoundTags { get; } = [];
    public ObservableCollection<TagViewModel> AddedTags { get; } = [];

    public bool IsYourGamesPage => CurrentMode == LibraryViewMode.YourGames;
    public bool IsFavoritePage => CurrentMode == LibraryViewMode.Favorites;
    public bool IsMoreGamesPage => CurrentMode == LibraryViewMode.MoreGames;

    public bool HasNoFoundTags => FoundTags.Count == 0;
    public bool HasAddedTags => AddedTags.Count > 0;
    public bool HasNoTags => !AddedTags.Any() && !FoundTags.Any();

    public bool HasFoundGames => FoundGames.Count == 0;

    private List<long> _gamePoolIds = [];
    private List<Tag> _allAvailableTags = [];

    private readonly ICacheRefresher _cacheRefresher;
    private readonly INavigator _navigator;
    private readonly IGameDataService _gameDataService;
    private readonly ICacheProvider _cacheProvider;
    private readonly IImageProvider _imageService;

    public LibraryViewModel(
        ICacheRefresher cacheRefresher,
        INavigator navigator,
        IGameDataService gameDataService,
        ICacheProvider cacheProvider,
        IImageProvider imageService)
    {
        _cacheRefresher = cacheRefresher;
        _navigator = navigator;
        _gameDataService = gameDataService;
        _cacheProvider = cacheProvider;
        _imageService = imageService;

        _cacheRefresher.OnRefreshAll += RefreshPage;
        UpdateTags();
    }

    [RelayCommand]
    public void RefreshPage()
    {
        _ = RefreshLibrary();
    }

    [RelayCommand]
    public void ChangeView(LibraryViewMode mode)
    {
        CurrentMode = mode;

        OnPropertyChanged(nameof(IsYourGamesPage));
        OnPropertyChanged(nameof(IsFavoritePage));
        OnPropertyChanged(nameof(IsMoreGamesPage));

        _gamePoolIds = [.. mode switch
        {
            LibraryViewMode.YourGames => _gameDataService.GetLibraryGames(),
            LibraryViewMode.Favorites => _gameDataService.GetFavoriteGames(),
            LibraryViewMode.MoreGames => _gameDataService.GetAllGames(),
            _ => throw new NotSupportedException($"Unsupported library view mode: {mode}")
        }];

        _ = UpdateImages();
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
        IEnumerable<long> filteredIds = _gamePoolIds;

        if (!string.IsNullOrWhiteSpace(SearchGames))
        {
            filteredIds = filteredIds.Where(id =>
                _cacheProvider.GetDisplayOf(id)?.Title?
                    .Contains(SearchGames, StringComparison.OrdinalIgnoreCase) ?? false
            );
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

        _ = RefreshFoundGames(FoundGames, [.. filteredIds]);
        OnPropertyChanged(nameof(HasFoundGames));
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

    private async Task RefreshLibrary()
    {
        await UpdateImages();
        UpdateTags();
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

    private async Task UpdateImages()
    {
        _gamePoolIds = [.. CurrentMode switch
        {
            LibraryViewMode.YourGames => _gameDataService.GetLibraryGames(),
            LibraryViewMode.Favorites => _gameDataService.GetFavoriteGames(),
            LibraryViewMode.MoreGames => _gameDataService.GetAllGames(),
            _ => throw new NotSupportedException($"Unsupported library view mode: {CurrentMode}")
        }];

        await RefreshFoundGames(FoundGames, [.. _gamePoolIds]);
    }

    private async Task RefreshFoundGames(
        ObservableCollection<GameButtonViewModel> found,
        List<long> targetIds)
    {
        List<GameButtonViewModel> toRemove = [.. found
            .Where(vm => !targetIds.Contains(vm.GameId))];

        foreach (var vm in toRemove)
        {
            found.Remove(vm);
        }

        List<Task> tasks = [];
        foreach (var id in targetIds)
        {
            if (found.Any(bvm => bvm.GameId == id)) continue;

            var bvm = new GameButtonViewModel(_imageService, _navigator) { Id = id };
            found.Add(bvm);

            GameDisplay display = _cacheProvider.GetDisplayOf(id);

            if (display?.IconUrl != null)
            {
                Task task = bvm.SetActiveLookAsync(display.IconUrl);
                tasks.Add(task);
            }
        }

        await Task.WhenAll(tasks);

        OnPropertyChanged(nameof(HasFoundGames));
    }
}
