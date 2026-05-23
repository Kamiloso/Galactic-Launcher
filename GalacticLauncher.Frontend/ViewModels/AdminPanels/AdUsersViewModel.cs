using CommunityToolkit.Mvvm.ComponentModel;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.AdminPanels;

internal class AdUsersViewModel(INavigator navigator) : ObservableObject
{
    public void ShowUsers()
    {
        navigator.AdminPanelNavigateTo<AdUsersViewModel>();
    }
}
