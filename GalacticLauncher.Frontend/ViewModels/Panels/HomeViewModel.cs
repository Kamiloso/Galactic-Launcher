using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.ViewModels.Buttons;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class HomeViewModel : ObservableObject
{
    private const int LIB_CAPACITY = 4;
    private const int FAV_CAPACITY = 3;

    private readonly ICacheRefresher _cacheRefresher;
    private readonly IGameDataService _gameDataService;
    private readonly IGameButtonFactory _gameButtonFactory;

    public ObservableCollection<GameButtonViewModel> Recommendations { get; } = [];
    public ObservableCollection<GameButtonViewModel> Favourites { get; } = [];

    [ObservableProperty]
    private GameButtonViewModel? _lastAdded;

    public HomeViewModel(
        ICacheRefresher cacheRefresher,
        IGameDataService gameDataService,
        IGameButtonFactory gameButtonFactory)
    {
        _cacheRefresher = cacheRefresher;
        _gameDataService = gameDataService;
        _gameButtonFactory = gameButtonFactory;

        _cacheRefresher.OnRefreshAll += UpdateImages;

        UpdateImages();
    }

    [RelayCommand]
    public void RefreshPage()
    {
        _ = _cacheRefresher.RefreshAll();
    }

    private void UpdateImages()
    {
        RefreshRecommendations();
        RefreshFavorites();
        RefreshLastAdded();
    }

    private void RefreshRecommendations()
    {
        Recommendations.Clear();

        List<long> list = [.. _gameDataService
            .ObtainAllRecommendations(LIB_CAPACITY)];

        for (int i = 0; i < LIB_CAPACITY; i++)
        {
            Recommendations.Add(i < list.Count
                ? _gameButtonFactory.CreateAndStartLoading(list[i])
                : _gameButtonFactory.CreateEmpty());
        }
    }

    private void RefreshFavorites()
    {
        Favourites.Clear();

        List<long> list = [.. _gameDataService
            .ObtainFavoriteRecommendations(FAV_CAPACITY)];

        for (int i = 0; i < FAV_CAPACITY; i++)
        {
            Favourites.Add(i < list.Count
                ? _gameButtonFactory.CreateAndStartLoading(list[i])
                : _gameButtonFactory.CreateEmpty());
        }
    }

    private void RefreshLastAdded()
    {
        List<long?> libGames = [.. _gameDataService.GetLibraryGames()];

        long? last = libGames.FirstOrDefault();

        LastAdded = last != null
            ? _gameButtonFactory.CreateAndStartLoading(last.Value)
            : _gameButtonFactory.CreateEmpty();
    }
}
