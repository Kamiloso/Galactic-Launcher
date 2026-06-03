using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.Services.Images;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.Controls;

internal partial class LibraryGameButtonViewModel(
    IImageProvider imageProvider,
    IGameListManager gameListManager,
    INavigator navigator) : ObservableObject
{
    private const string EMPTY_STATUS = "";
    private const string GAME_NOT_FOUND = "NO GAME";
    private const string LOADING_IMAGE = "LOADING IMAGE...";
    private const string IMAGE_NOT_FOUND = "IMAGE NOT FOUND";

    [ObservableProperty]
    private bool _isGameValid;

    [ObservableProperty]
    private long _gameId;

    [ObservableProperty]
    private string _statusMessage = EMPTY_STATUS;

    [ObservableProperty]
    private Bitmap? _icon;

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

    public required long? Id
    {
        get => IsGameValid ? GameId : null;
        init
        {
            IsGameValid = value.HasValue;
            GameId = value ?? 0;

            if (IsGameValid)
            {
                InitializeGameStates();
            }
        }
    }

    [RelayCommand]
    public void ShowGame()
    {
        navigator.NavigateTo<GameViewModel>(GameId);
    }

    public void SetInactiveLook()
    {
        StatusMessage = GAME_NOT_FOUND;
    }

    [RelayCommand]
    public void ToggleFavorite()
    {
        if (IsFav)
        {
            gameListManager.RemoveFromFavorite(GameId);
            IsFav = false;
        }
        else
        {
            gameListManager.AddToFavorite(GameId);
            IsFav = true;
            IsLib = true;
        }

        UpdateIcons();
    }

    [RelayCommand]
    public void ToggleLibrary()
    {
        if (IsLib)
        {
            gameListManager.RemoveFromLibrary(GameId);
            IsLib = false;
            IsFav = false;
        }
        else
        {
            gameListManager.AddToLibrary(GameId);
            IsLib = true;
        }

        UpdateIcons();
    }

    private void UpdateIcons()
    {
        IconFav = GetResourceGeometry(IsFav ? "IconFav" : "IconNotFav");

        IconLib = GetResourceGeometry(IsLib ? "IconLib" : "IconNotLib");
    }

    private Geometry? GetResourceGeometry(string v)
    {
        if (Application.Current?.TryFindResource(v, out var res) == true && res is Geometry geo)
        {
            return geo;
        }
        return null;
    }

    private void InitializeGameStates()
    {
        IsLib = gameListManager.GetLibraryGames().ToList().Contains(GameId);
        IsFav = gameListManager.GetFavoriteGames().ToList().Contains(GameId);

        UpdateIcons();
    }

    public async Task SetActiveLookAsync(string? url)
    {
        if (url == null)
        {
            StatusMessage = IMAGE_NOT_FOUND;
            return;
        }

        StatusMessage = LOADING_IMAGE;

        try
        {
            string filePath = await imageProvider.GetImagePathAsync(url);

            Icon = new Bitmap(filePath);
            StatusMessage = EMPTY_STATUS;
        }
        catch (DownloadException)
        {
            StatusMessage = IMAGE_NOT_FOUND;
        }
    }
}
