using System.ComponentModel;
using GalacticLauncher.Frontend.ViewModels.MainPanelViewModels;
using GalacticLauncher.Frontend.ViewModels.Interfaces;

namespace GalacticLauncher.Frontend.ViewModels.AdminViewModels
{
    internal class AllGamesViewModel: INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly AdminViewModel _adminViewModel;

        public void OnActivated()
        {
        }
        public AllGamesViewModel(AdminViewModel adminViewModel)
        {
            _adminViewModel = adminViewModel;
        }

        public void ShowAllGames()
        {
            _adminViewModel.ShowAllGames();
        }
    }
}
