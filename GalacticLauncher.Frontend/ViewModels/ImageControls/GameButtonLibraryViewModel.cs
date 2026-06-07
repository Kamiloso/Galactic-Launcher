using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.ImageControls;

internal partial class GameButtonLibraryViewModel : GameButtonViewModel
{
    [ObservableProperty]
    private string _gameTitle = EMPTY_STATUS;

    [ObservableProperty]
    private string _gameAuthor = EMPTY_STATUS;

    [ObservableProperty]
    private string _gameDescription = EMPTY_STATUS;

    public bool InLibrary => _gameListManager.InLibrary(GameId);
    public bool InFavorite => _gameListManager.InFavorite(GameId);

    public ObservableCollection<Tag> Tags { get; } = [];

    private readonly IGameListManager _gameListManager;
    private readonly ICacheProvider _cacheProvider;

    public GameButtonLibraryViewModel(
        IImageProvider imageProvider,
        IGameListManager gameListManager,
        ICacheProvider cacheProvider,
        INavigator navigator) : base(imageProvider, navigator)
    {
        _gameListManager = gameListManager;
        _cacheProvider = cacheProvider;

        _gameListManager.OnListsChanged += RefreshIcons;

        RefreshIcons();
        LoadTags();
    }

    private void RefreshIcons()
    {
        OnPropertyChanged(nameof(InLibrary));
        OnPropertyChanged(nameof(InFavorite));
    }

    private void LoadTags()
    {
        Tags.Clear();

        _cacheProvider.GetTagsOf(GameId)
            .ToList()
            .ForEach(Tags.Add);
    }

    public async Task SetActiveLookAsync(Game game)
    {
        GameTitle = game.Name;
        GameAuthor = game.Author;
        GameDescription = game.Description;

        string? url = game.IconUrl;
        await SetActiveLookAsync(url);
    }

    [RelayCommand]
    public void ToggleFavorite()
    {
        Action<long> action = InFavorite
            ? _gameListManager.RemoveFromFavorite
            : _gameListManager.AddToFavorite;

        action(GameId);
    }

    [RelayCommand]
    public void ToggleLibrary()
    {
        Action<long> action = InLibrary
            ? _gameListManager.RemoveFromLibrary
            : _gameListManager.AddToLibrary;

        action(GameId);
    }
}
