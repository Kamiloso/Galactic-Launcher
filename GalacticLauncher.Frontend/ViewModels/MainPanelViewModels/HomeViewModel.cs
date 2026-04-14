using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalacticLauncher.Frontend.ViewModels.Interfaces;
using GalacticLauncher.Frontend.ViewModels.MainWindowViewModels;

namespace GalacticLauncher.Frontend.ViewModels.MainPanelViewModels
{
    internal class HomeViewModel : INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly MainWindowViewModel _mainWindowViewModel;

        public void OnActivated()
        {
        }
        public HomeViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public void ShowGame()
        {
            _mainWindowViewModel.ShowGame();
        }

    }
}