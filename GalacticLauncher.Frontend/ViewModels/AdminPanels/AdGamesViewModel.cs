using CommunityToolkit.Mvvm.ComponentModel;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.AdminPanels;

internal class AdGamesViewModel(INavigator navigator) : ObservableObject
{
    public void ShowAllGames()
    {
        navigator.AdminPanelNavigateTo<AdGamesViewModel>();
    }
}
