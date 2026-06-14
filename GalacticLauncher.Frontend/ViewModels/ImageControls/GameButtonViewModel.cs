using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.ViewModels.ImageLoad;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.ImageControls;

public abstract partial class GameButtonViewModel(
    IImageProvider imageProvider,
    INavigator navigator) : ImageViewModel(imageProvider)
{
    [ObservableProperty]
    private long _gameId;

    public virtual required long Id
    {
        init => GameId = value;
    }

    [RelayCommand]
    public void ShowGame()
    {
        navigator.NavigateTo<GameViewModel>(GameId);
    }
}
