using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Files;
using GalacticLauncher.Frontend.ViewModels.Buttons;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class HomeViewModel: ObservableObject
{
    private readonly IGameSelectionService _gameSelectionService;
    private readonly INavigator _navigator;
    private readonly IImageService _imageService;
    private readonly ICacheProvider _cacheProvider;
    private readonly ICacheRefresher _cacheRefresher;

    public ObservableCollection<GameButtonViewModel> Recommendations { get; } = [];
    public ObservableCollection<GameButtonViewModel> Favourites { get; } = [];

    [ObservableProperty]
    private GameButtonViewModel? _lastAdded;

    public HomeViewModel(
        IImageService imageService, 
        INavigator navigator,
        IGameSelectionService gameSelectionService,
        ICacheProvider cacheProvider,
        ICacheRefresher cacheRefresher
        )
    {
        _navigator = navigator;
        _imageService = imageService;
        _gameSelectionService = gameSelectionService;
        _cacheProvider = cacheProvider;
        _cacheRefresher = cacheRefresher;

        _cacheRefresher.OnRefreshAll += async () => await UpdateImages();

        _ = _cacheRefresher.RefreshAll();
        _ = UpdateImages();
    }

    private async Task UpdateImages()
    {
        var recommended = _gameSelectionService.GetRecommendedGames(4).ToList();
        var favorite = _gameSelectionService.GetFavoriteGames(3).ToList();
        var lastAddedId = _gameSelectionService.GetLastAddedLibraryGame();

        await RefreshCollections(Recommendations, recommended, 4);
        await RefreshCollections(Favourites, favorite, 3);

        if (lastAddedId == null)
        {
            LastAdded = new GameButtonViewModel(-1, _imageService, _navigator);
        }
        else if (LastAdded?.GameId != lastAddedId)
        {
            var vm = new GameButtonViewModel(lastAddedId.Value, _imageService, _navigator);
            var display = _cacheProvider.GetDisplayOf(lastAddedId.Value);
            if (display?.IconUrl != null)
                await vm.LoadAsync(display.IconUrl);

            LastAdded = vm;
        }
    }

    private async Task RefreshCollections(ObservableCollection<GameButtonViewModel> collection, List<long> targetIds, int capacity)
    {
        var toRemove = collection.Where(vm => !targetIds.Contains(vm.GameId)).ToList();
        foreach (var vm in toRemove) collection.Remove(vm);

        if (targetIds.Count == 0)
        {
            for (int i = 0; i < capacity; i++)
            {
                collection.Add(new GameButtonViewModel(-1, _imageService, _navigator));
            }
            return;
        }

        var tasks = new List<Task>();
        foreach (var id in targetIds)
        {
            if (collection.Any(vm => vm.GameId == id)) continue;

            var vm = new GameButtonViewModel(id, _imageService, _navigator);
            collection.Add(vm);

            var display = _cacheProvider.GetDisplayOf(id);
            if (display?.IconUrl != null)
                tasks.Add(vm.LoadAsync(display.IconUrl));
        }

        await Task.WhenAll(tasks);
    }

    [RelayCommand]
    public async Task RefreshPage() => await UpdateImages();
}
