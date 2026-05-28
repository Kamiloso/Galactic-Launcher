using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Services.Images;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.Buttons;

internal partial class GameButtonViewModel(
    IImageProvider imageProvider,
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

    public required long? Id
    {
        get => IsGameValid ? GameId : null;
        init
        {
            IsGameValid = value.HasValue;
            GameId = value ?? 0;
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
