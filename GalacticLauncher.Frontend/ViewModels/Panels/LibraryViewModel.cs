using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Files;
using GalacticLauncher.Frontend.Services.UserData;
using GalacticLauncher.Frontend.ViewModels.Buttons;
using GalacticLauncher.Frontend.ViewModels.Tags;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class LibraryViewModel : ObservableObject
{

    private readonly ICacheRefresher _cacheRefresher;
    private readonly INavigator _navigator;
    private readonly IUserDataService _userDataService;
    private readonly ICacheProvider _cacheProvider;
    private readonly IImageService _imageService;

    public LibraryViewModel(
        ICacheRefresher cacheRefresher,
        INavigator navigator,
        IUserDataService userDataService,
        ICacheProvider cacheProvider,
        IImageService imageService
        )
    {
        _cacheRefresher = cacheRefresher;
        _navigator = navigator;
        _userDataService = userDataService;
        _cacheProvider = cacheProvider;
        _imageService = imageService;



        _cacheRefresher.OnRefreshAll += RefreshLibrary;
        UpdateTags();
    }
    public enum LibraryViewMode
    {
        YourGames,
        Favorites,
        MoreGames
    }

    public bool IsYourGamesPage => CurrentMode == LibraryViewMode.YourGames;
    public bool IsFavoritePage => CurrentMode == LibraryViewMode.Favorites;
    public bool IsMoreGamesPage => CurrentMode == LibraryViewMode.MoreGames;

    [ObservableProperty]
    private LibraryViewMode _currentMode = LibraryViewMode.YourGames;

    [ObservableProperty]
    private string? _searchGames;

    [ObservableProperty]
    private string? _searchTags;

    private IEnumerable<long> _gamePoolIds;
    public ObservableCollection<GameButtonViewModel> FoundGames { get; } = [];
    public ObservableCollection<TagViewModel> FoundTags { get; } = [];
    public ObservableCollection<TagViewModel> AddedTags { get; } = [];

    private IEnumerable<Tag> _allAvailableTags;

    public bool HasNoFoundTags => FoundTags.Count == 0;
    public bool HasAddedTags => AddedTags.Count > 0;
    public bool HasNoTags => !AddedTags.Any() && !FoundTags.Any();

    public bool HasFoundGames => FoundGames.Count == 0;
    partial void OnSearchGamesChanged(string? value) => ApplyFilters();

    partial void OnSearchTagsChanged(string? value)
    {
        var searchText = value?.ToLower() ?? "";

        var activeIds = AddedTags.Select(t => t.Id).ToHashSet();
        var potentialTags = _allAvailableTags.Where(t => !activeIds.Contains(t.Id));

        var filtered = string.IsNullOrWhiteSpace(searchText)
            ? potentialTags
            : potentialTags.Where(t => t.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));

        var toRemove = FoundTags.Where(vm => !filtered.Any(t => t.Id == vm.Id)).ToList();
        foreach (var vm in toRemove) FoundTags.Remove(vm);

        foreach (var tag in filtered)
        {
            if (!FoundTags.Any(vm => vm.Id == tag.Id))
            {
                FoundTags.Add(new TagViewModel(tag, ToggleTag));
            }
        }
    }

    [RelayCommand]
    public async Task ChangeView(LibraryViewMode mode)
    {
        CurrentMode = mode;

        OnPropertyChanged(nameof(IsYourGamesPage));
        OnPropertyChanged(nameof(IsFavoritePage));
        OnPropertyChanged(nameof(IsMoreGamesPage));

        _gamePoolIds = mode switch
        {
            LibraryViewMode.Favorites => _userDataService.GetFavoriteIds(),
            LibraryViewMode.MoreGames => _cacheProvider.AllGameIds(),
            _ => _userDataService.GetLibraryIds(),
        };
        await UpdateImages();
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
                var gameTags = _cacheProvider.GetTagsForGame(id);
                return activeTagIds.All(tId => gameTags.Contains(tId));
            });
        }

        _ = RefreshFoundGames(FoundGames, filteredIds.ToList());
        OnPropertyChanged(nameof(HasFoundGames));
    }
    private void ToggleTag(TagViewModel tagVm)
    {
        if (AddedTags.Contains(tagVm))
        {
            AddedTags.Remove(tagVm);
            tagVm.IsActive = false;
            if (!FoundTags.Contains(tagVm)) FoundTags.Add(tagVm);
        }
        else
        {
            FoundTags.Remove(tagVm);
            tagVm.IsActive = true;
            AddedTags.Add(tagVm);
        }

        
        OnPropertyChanged(nameof(HasNoTags));
        OnPropertyChanged(nameof(HasNoFoundTags));
        OnPropertyChanged(nameof(HasAddedTags));

        ApplyFilters();
    }

    private async void RefreshLibrary()
    {
        await UpdateImages();
        UpdateTags();
    }

    private void UpdateTags()
    {
        _allAvailableTags = _cacheProvider.GetAllTags().ToList();
        RefreshTagsList(FoundTags);
        RefreshTagsList(AddedTags);
    }
    private void RefreshTagsList(ObservableCollection<TagViewModel> targetCollection)
    {
        var currentAvailableTags = _cacheProvider.GetAllTags().ToDictionary(t => t.Id);

        var toRemove = targetCollection.Where(vm => !currentAvailableTags.ContainsKey(vm.Id)).ToList();
        foreach (var vm in toRemove) targetCollection.Remove(vm);

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
        _gamePoolIds = CurrentMode switch
        {
            LibraryViewMode.Favorites => _userDataService.GetFavoriteIds(),
            LibraryViewMode.MoreGames => _cacheProvider.AllGameIds(),
            _ => _userDataService.GetLibraryIds(),
        };

        await RefreshFoundGames(FoundGames, [.. _gamePoolIds]);
    }

    private async Task RefreshFoundGames(ObservableCollection<GameButtonViewModel> found, List<long> targetIds)
    {
        var toRemove = found.Where(vm => !targetIds.Contains(vm.GameId)).ToList();
        foreach (var vm in toRemove) found.Remove(vm);

        if (targetIds.Count == 0) return;

        var tasks = new List<Task>();
        foreach (var id in targetIds)
        {
            if (found.Any(vm => vm.GameId == id)) continue;

            var vm = new GameButtonViewModel(id, _imageService, _navigator);
            found.Add(vm);

            var display = _cacheProvider.GetDisplayOf(id);
            if (display?.IconUrl != null)
                tasks.Add(vm.LoadAsync(display.IconUrl));
        }

        await Task.WhenAll(tasks);
        OnPropertyChanged(nameof(HasFoundGames));
    }

    [RelayCommand]
    public async Task RefreshPage() => RefreshLibrary();
}
