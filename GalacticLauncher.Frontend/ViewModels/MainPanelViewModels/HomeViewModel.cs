using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.ViewModels.MainWindowViewModels;
using System.ComponentModel;

namespace GalacticLauncher.Frontend.ViewModels.MainPanelViewModels;

internal class HomeViewModel(MainWindowViewModel mainWindowViewModel) : INotifyPropertyChanged, INavigationAware
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnActivated()
    {
        ;
    }

    public void ShowGame()
    {
        mainWindowViewModel.ShowGame();
    }
}