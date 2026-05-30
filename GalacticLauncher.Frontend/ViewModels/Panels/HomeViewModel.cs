using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.ViewModels.Buttons;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class HomeViewModel : ObservableObject
{
    private const int LIB_CAPACITY = 4;
    private const int FAV_CAPACITY = 3;

    private readonly ICacheRefresher _cacheRefresher;
    private readonly IGameListManager _gameListManager;
    private readonly ILastGameManager _lastGameManager;
    private readonly IGameButtonFactory _gameButtonFactory;

    public ObservableCollection<GameButtonViewModel> Recommendations { get; } = [];
    public ObservableCollection<GameButtonViewModel> Library { get; } = [];

    [ObservableProperty]
    private GameButtonViewModel? _recent;

    public HomeViewModel(
        ICacheRefresher cacheRefresher,
        IGameListManager gameListManager,
        ILastGameManager lastGameManager,
        IGameButtonFactory gameButtonFactory)
    {
        _cacheRefresher = cacheRefresher;
        _gameListManager = gameListManager;
        _lastGameManager = lastGameManager;
        _gameButtonFactory = gameButtonFactory;

        _cacheRefresher.OnRefreshAll += UpdateImages;
    }

    [RelayCommand]
    public void RefreshPage()
    {
        UpdateImages();
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

        List<long> list = [.. _gameListManager
            .ObtainNolibRecommendations(LIB_CAPACITY)];

        for (int i = 0; i < LIB_CAPACITY; i++)
        {
            Recommendations.Add(i < list.Count
                ? _gameButtonFactory.CreateAndStartLoading(list[i])
                : _gameButtonFactory.CreateEmpty());
        }
    }

    private void RefreshFavorites()
    {
        Library.Clear();

        List<long> list = [.. _gameListManager
            .ObtainLibraryRecommendations(FAV_CAPACITY)];

        for (int i = 0; i < FAV_CAPACITY; i++)
        {
            Library.Add(i < list.Count
                ? _gameButtonFactory.CreateAndStartLoading(list[i])
                : _gameButtonFactory.CreateEmpty());
        }
    }

    private void RefreshLastAdded()
    {
        long? last = _lastGameManager.GetLastGame();

        Recent = last != null
            ? _gameButtonFactory.CreateAndStartLoading(last.Value)
            : _gameButtonFactory.CreateEmpty();
    }
}
