using System.ComponentModel;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.ViewModels.MainPanelViewModels;

namespace GalacticLauncher.Frontend.ViewModels.MainWindowViewModels;

internal class MainWindowViewModel : NotifierBase, INotifyPropertyChanged
{
    private object? _currentPage;
    public object? CurrentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentActivePage));
        }
    }
    public string CurrentActivePage => CurrentPage?.GetType().Name ?? "";

    private readonly HomeViewModel _homePage;
    private readonly LibraryViewModel _libraryPage;
    private readonly GameViewModel _gamePage;
    private readonly AdminViewModel _adminPage;

    public MainWindowViewModel()
    {
        _homePage = new HomeViewModel(this);
        _libraryPage = new LibraryViewModel(this);
        _gamePage = new GameViewModel();
        _adminPage = new AdminViewModel();

        CurrentPage = _homePage;
    }

    public void ShowHome() => SetPage(_homePage);
    public void ShowLibrary() => SetPage(_libraryPage);
    public void ShowGame() => SetPage(_gamePage);
    public void ShowAdmin() => SetPage(_adminPage);

    private void SetPage(object page)
    {
        CurrentPage = page;

        if (page is INavigationAware nav)
        {
            nav.OnActivated();
        }
    }
}
