using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.ViewModels.Controls;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class LibraryViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _searchGames;

    [ObservableProperty]
    private string? _searchTags;

    public ObservableCollection<GameButtonViewModel> GameControls { get; } = [];

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

    public LibraryViewModel(
        ICacheRefresher cacheRefresher,
        IGameListManager gameListManager,
        IGameButtonFactory gameButtonFactory)
    {
        _cacheRefresher = cacheRefresher;
        _gameListManager = gameListManager;
        _gameButtonFactory = gameButtonFactory;

        _cacheRefresher.OnInitialize += RefreshPage;
    }

    [RelayCommand]
    public void RefreshPage()
    {
        LoadGamesForMode(CurrentMode);
    }

    [RelayCommand]
    public void ChangeViewCommand(LibraryViewMode mode)
    {
        CurrentMode = mode;
    }

    partial void OnCurrentModeChanged(LibraryViewMode value)
    {
        LoadGamesForMode(value);
    }

    private void LoadGamesForMode(LibraryViewMode mode)
    {
        List<long> gameIdPool = [.. mode switch
        {
            LibraryViewMode.YourGames => _gameListManager.GetLibraryGames(),
            LibraryViewMode.Favorites => _gameListManager.GetFavoriteGames(),
            LibraryViewMode.MoreGames => _gameListManager.GetNolibGames(),
            _ => throw new NotSupportedException()
        }];

        GameControls.Clear();

        foreach (long id in gameIdPool)
        {
            var gbvm = _gameButtonFactory.CreateAndStartLoading(id);
            GameControls.Add(gbvm);
        }
    }
}
