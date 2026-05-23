using CommunityToolkit.Mvvm.ComponentModel;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.AdminPanels;

internal class AdTagsViewModel(INavigator navigator) : ObservableObject
{
    public void ShowTags()
    {
        navigator.AdminPanelNavigateTo<AdTagsViewModel>();
    }
}
