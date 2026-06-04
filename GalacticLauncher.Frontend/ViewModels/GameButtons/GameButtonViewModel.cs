using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.ViewModels.GameButtons;

internal abstract partial class GameButtonViewModel(
    IImageProvider imageProvider,
    INavigator navigator) : ObservableObject
{
    protected const string EMPTY_STATUS = "";
    protected const string GAME_NOT_FOUND = "NO GAME";
    protected const string LOADING_IMAGE = "LOADING IMAGE...";
    protected const string IMAGE_NOT_FOUND = "IMAGE NOT FOUND";

    [ObservableProperty]
    private long _gameId;

    [ObservableProperty]
    private string _statusMessage = EMPTY_STATUS;

    [ObservableProperty]
    private Bitmap? _icon;

    public virtual required long Id
    {
        get => GameId;
        init => GameId = value;
    }

    [RelayCommand]
    public void ShowGame()
    {
        navigator.NavigateTo<GameViewModel>(GameId);
    }

    public virtual void SetInactiveLook()
    {
        StatusMessage = GAME_NOT_FOUND;
    }

    public virtual async Task SetActiveLookAsync(Game? game)
    {
        string? url = game?.IconUrl;

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
