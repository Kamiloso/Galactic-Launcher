using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.GameButtons;

internal partial class GameButtonLibraryViewModel(
    IImageProvider imageProvider,
    IGameListManager gameListManager,
    ICacheProvider cacheProvider,
    INavigator navigator) : GameButtonViewModel(imageProvider, navigator)
{
    [ObservableProperty]
    private string _gameTitle = EMPTY_STATUS;

    [ObservableProperty]
    private string _gameAuthor = EMPTY_STATUS;

    [ObservableProperty]
    private string _gameDescription = EMPTY_STATUS;

    [ObservableProperty]
    private bool _isFav;

    [ObservableProperty]
    private bool _isLib;

    [ObservableProperty]
    private Geometry? _iconLib;

    [ObservableProperty]
    private Geometry? _iconFav;

    public ObservableCollection<Tag> Tags { get; } = [];

    public override required long Id
    {
        get => base.Id;
        init
        {
            base.Id = value;
            RefreshState();
        }
    }

    [RelayCommand]
    public void ToggleFavorite()
    {
        Action<long> action = IsFav
            ? gameListManager.RemoveFromFavorite
            : gameListManager.AddToFavorite;

        action(GameId);

        RefreshState();
    }

    [RelayCommand]
    public void ToggleLibrary()
    {
        Action<long> action = IsLib
            ? gameListManager.RemoveFromLibrary
            : gameListManager.AddToLibrary;

        action(GameId);

        RefreshState();
    }

    public override async Task SetActiveLookAsync(Game? game)
    {
        GameTitle = game?.Name ?? "";
        GameAuthor = game?.Author ?? "";
        GameDescription = game?.Description ?? "";

        await base.SetActiveLookAsync(game);
    }

    private static Geometry? GetResourceGeometry(string v)
    {
        return Application.Current?.TryFindResource(v, out var res) == true &&
               res is Geometry geo ? geo : null;
    }

    private void RefreshState()
    {
        IsLib = gameListManager.GetLibraryGames().Contains(GameId);
        IsFav = gameListManager.GetFavoriteGames().Contains(GameId);

        IconLib = GetResourceGeometry(IsLib ? "IconLib" : "IconNotLib");
        IconFav = GetResourceGeometry(IsFav ? "IconFav" : "IconNotFav");

        LoadTags();
    }

    private void LoadTags()
    {
        Tags.Clear();

        List<Tag> gameTags = [.. cacheProvider.GetTagsByGameId(GameId)];

        foreach (Tag tag in gameTags)
        {
            Tags.Add(tag);
        }
    }
}
