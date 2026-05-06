using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GalacticLauncher.Frontend.ViewModels.Interfaces;
using GalacticLauncher.Frontend.ViewModels.MainWindowViewModels;
using GalacticLauncher.Frontend.Models;

namespace GalacticLauncher.Frontend.ViewModels.MainPanelViewModels
{
    internal class LibraryViewModel: INotifyPropertyChanged, INavigationAware
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly MainWindowViewModel _mainWindowViewModel;

        //only temporary okay
        private readonly string _allGames = "no";
        private readonly int _favourite = 1;


        private object? _currentMode;
        public object? CurrentMode
        {
            get => _currentMode;
            set
            {
                _currentMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentActiveMode));
            }
        }
        public string CurrentActiveMode => CurrentMode?.GetType().Name ?? "";
        public LibraryViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }
        public void OnActivated()
        {
        }
        public void ShowFavourites()
        {
            _currentMode = _favourite;
        }

        public void ShowAllGames()
        {
            _currentMode = _allGames;
        }
        public void ShowGame()
        {
            _mainWindowViewModel.ShowGame();
        }
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
