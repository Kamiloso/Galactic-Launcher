using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalacticLauncher.Frontend.ViewModels.Interfaces;

namespace GalacticLauncher.Frontend.ViewModels.MainPanelViewModels
{
    internal class LibraryViewModel: INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnActivated()
        {
        }
    }
}
