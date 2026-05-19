using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.Services;

namespace GalacticLauncher.Frontend.ViewModels.AdminPanels;

internal class AdUsersViewModel(Navigator navigator) : NotifierBase
{
    public void ShowUsers()
    {
        navigator.AdminPanelNavigateTo<AdUsersViewModel>();
    }
}
