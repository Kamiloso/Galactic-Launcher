using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.Services;

namespace GalacticLauncher.Frontend.ViewModels.AdminPanels;

internal class AdGamesViewModel(Navigator navigator) : NotifierBase
{
    public void ShowAllGames()
    {
        navigator.AdminPanelNavigateTo<AdGamesViewModel>();
    }
}
