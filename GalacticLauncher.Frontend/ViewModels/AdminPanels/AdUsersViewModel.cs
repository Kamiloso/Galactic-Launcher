using CommunityToolkit.Mvvm.ComponentModel;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.Services;

namespace GalacticLauncher.Frontend.ViewModels.AdminPanels;

internal class AdUsersViewModel(Navigator navigator) : ObservableObject
{
    public void ShowUsers()
    {
        navigator.AdminPanelNavigateTo<AdUsersViewModel>();
    }
}
