using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.Services;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal class HomeViewModel(Navigator navigator) : NotifierBase
{
    public void ShowGame()
    {
        navigator.NavigateTo<GameViewModel>();
    }
}
