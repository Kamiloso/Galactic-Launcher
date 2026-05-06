using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalacticLauncher.Frontend.ViewModels.Interfaces;
using GalacticLauncher.Frontend.ViewModels.MainPanelViewModels;

namespace GalacticLauncher.Frontend.ViewModels.AdminViewModels
{
    internal class UsersViewModel : INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly AdminViewModel _adminViewModel;

        public void OnActivated()
        {
        }
        public UsersViewModel(AdminViewModel adminViewModel)
        {
            _adminViewModel = adminViewModel;
        }

        public void ShowUsers()
        {
            _adminViewModel.ShowUsers();
        }
    }
}