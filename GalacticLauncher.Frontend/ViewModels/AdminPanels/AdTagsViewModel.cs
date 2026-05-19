using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.Services;

namespace GalacticLauncher.Frontend.ViewModels.AdminPanels;

internal class AdTagsViewModel(Navigator navigator) : NotifierBase
{
    public void ShowTags()
    {
        navigator.AdminPanelNavigateTo<AdTagsViewModel>();
    }
}
