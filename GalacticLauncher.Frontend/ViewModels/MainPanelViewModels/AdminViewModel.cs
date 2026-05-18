using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.ViewModels.AdminViewModels;

namespace GalacticLauncher.Frontend.ViewModels.MainPanelViewModels;

internal class AdminViewModel : NotifierBase, INavigationAware
{
    private TagsViewModel? _tagsPage;
    private UsersViewModel? _usersPage;
    private AllGamesViewModel? _allGamesPage;

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

    public AdminViewModel()
    {
        ShowTags();
    }

    public void OnActivated()
    {
        ;
    }

    private void SetCurrentPage(object page)
    {
        CurrentPage = page;

        if (page is INavigationAware nav)
        {
            nav.OnActivated();
        }
    }

    public void ShowAllGames()
    {
        _allGamesPage ??= new AllGamesViewModel(this);
        SetCurrentPage(_allGamesPage);
    }

    public void ShowTags()
    {
        _tagsPage ??= new TagsViewModel(this);
        SetCurrentPage(_tagsPage);
    }

    public void ShowUsers()
    {
        _usersPage ??= new UsersViewModel(this);
        SetCurrentPage(_usersPage);
    }
}

