using CommunityToolkit.Mvvm.ComponentModel;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.AdminPanels;

internal class AdTagsViewModel(Navigator navigator) : ObservableObject
{
    public void ShowTags()
    {
        navigator.AdminPanelNavigateTo<AdTagsViewModel>();
    }
}
