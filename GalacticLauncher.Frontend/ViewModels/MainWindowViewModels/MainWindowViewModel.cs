using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using GalacticLauncher.Core.DbRecords;
using GalacticLauncher.Frontend.Network;
using GalacticLauncher.Frontend.ViewModels.Interfaces;
using GalacticLauncher.Frontend.ViewModels.MainPanelViewModels;

namespace GalacticLauncher.Frontend.ViewModels.MainWindowViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private HomeViewModel? _homePage;
        private LibraryViewModel? _libraryPage;
        private GameViewModel? _gamePage;

        private object? _currentPage;
        public object? CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            CurrentPage = new HomeViewModel(this);
        }

        #region View navigation
        public void ShowHome()
        {
            _homePage ??= new HomeViewModel(this);
            SetCurrentPage(_homePage);
        }

        public void ShowLibrary()
        {
            _libraryPage ??= new LibraryViewModel();
            SetCurrentPage(_libraryPage);
        }

        public void ShowGame()
        {
            _gamePage ??= new GameViewModel();
            SetCurrentPage(_gamePage);
        }

        private void SetCurrentPage(object page)
        {
            CurrentPage = page;

            if (page is INavigationAware nav)
            {
                nav.OnActivated();
            }
        }
        #endregion

        // TODO: Rozdzielić rzeczy związane z Api do osobnego modelu


        // THIS IS A SECURITY CONCERN
        // ------------------------------
        // TODO: Use "#define DEBUG" to display a LARGE warning
        // indicating that the debug build is currently active!!!

        public required IApiService Api { get; init; }

        private string _responseJson = "";
        public string ResponseJson
        {
            get => _responseJson;
            set { _responseJson = value; OnPropertyChanged(); }
        }

        public async Task SendRequestAsync()
        {
            ResponseJson = "Pobieranie danych z serwera...";
            try
            {
                GameInfo gameInfo = await Api.PostJsonAsync<GameInfo, GameInfo>(
                    "testing/game-echo",
                    new GameInfo{ Id = 123, Name = "Space Eternity 3", Description = "Some Description..." }
                    );

                ResponseJson = JsonSerializer.Serialize(gameInfo);
            }
            catch (Exception ex)
            {
                ResponseJson = $"Wystąpił błąd:\n{ex.Message}";
            }
        }

        #region View Notifier

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}