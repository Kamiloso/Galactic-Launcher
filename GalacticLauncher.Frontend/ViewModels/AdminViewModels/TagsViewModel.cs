using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.ViewModels.MainPanelViewModels;

namespace GalacticLauncher.Frontend.ViewModels.AdminViewModels
{
    internal class TagsViewModel : INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly AdminViewModel _adminViewModel;

        public void OnActivated()
        {
        }
        public TagsViewModel(AdminViewModel adminViewModel)
        {
            _adminViewModel = adminViewModel;
        }

        public void ShowTags()
        {
            _adminViewModel.ShowTags();
        }
    }
}
