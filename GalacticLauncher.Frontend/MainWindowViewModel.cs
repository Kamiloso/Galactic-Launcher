using GalacticLauncher.Core.DbRecords;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Text.Json;
using GalacticLauncher.Frontend.Network;

namespace GalacticLauncher.Frontend
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
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