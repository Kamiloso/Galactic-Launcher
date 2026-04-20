using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalacticLauncher.Frontend.ViewModels.MainWindowViewModels;
using GalacticLauncher.Frontend.ViewModels.Interfaces;

namespace GalacticLauncher.Frontend.ViewModels.MainPanelViewModels
{
    internal class AdminViewModel : INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly MainWindowViewModel _mainWindowViewModel;

        public AdminViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }
        public void OnActivated()
        {
        }
    }
}

