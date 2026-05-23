using CommunityToolkit.Mvvm.ComponentModel;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal class LibraryViewModel : ObservableObject
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

    private readonly ICacheRefresher _cacheRefresher;
    private readonly INavigator _navigator;

    public LibraryViewModel(
        ICacheRefresher cacheRefresher,
        INavigator navigator)
    {
        _cacheRefresher = cacheRefresher;
        _navigator = navigator;

        _cacheRefresher.RefreshAll();
    }

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
        _navigator.NavigateTo<GameViewModel>(1); // TODO: pass game id
    }
}
