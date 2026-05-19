using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.Services;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal class LibraryViewModel(Navigator navigator) : NotifierBase
{
    //only temporary okay
    private readonly string _allGames = "no";
    private readonly int _favourite = 1;

    private object? _currentMode;
    public object? CurrentMode
    {
        get => _currentMode;
        set
        {
            _currentMode = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentActiveMode));
        }
    }

    public string CurrentActiveMode => CurrentMode?.GetType().Name ?? "";

    public void ShowFavourites()
    {
        _currentMode = _favourite;
    }

    public void ShowAllGames()
    {
        _currentMode = _allGames;
    }

    public void ShowGame()
    {
        navigator.NavigateTo<GameViewModel>();
    }
}
