using CommunityToolkit.Mvvm.ComponentModel;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.Services;

namespace GalacticLauncher.Frontend.ViewModels.AdminPanels;

internal class AdGamesViewModel(Navigator navigator) : ObservableObject
{
    public void ShowAllGames()
    {
        navigator.AdminPanelNavigateTo<AdGamesViewModel>();
    }
}
