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
    public GameButtonViewModel? LastAdded { get; private set; }

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


        _cacheRefresher.OnRefreshAll += RefreshHome;
        _ = InitializeAsync();
    }

    private void Clear()
    {
        Recommendations.Clear();
        Favourites.Clear();
        LastAdded = null;
    }

    private async Task InitializeAsync()
    {
        Clear();

        var recIds = _gameSelectionService.GetRecommendedGames(4);
        var loadTasks = new List<Task>();

        foreach (var id in recIds)
        {
            var recc = new GameButtonViewModel(id, _imageService, _navigator);
            Recommendations.Add(recc);

            var display = _cacheProvider.GetDisplayOf(id);
            if (display.IconUrl != null)
            {
                loadTasks.Add(recc.LoadAsync(display.IconUrl));
            }
        }
        await Task.WhenAll(loadTasks);
    }

    private async void RefreshHome()
    {
        await InitializeAsync();
    }


}
