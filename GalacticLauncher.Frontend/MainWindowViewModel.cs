using GalacticLauncher.Core.Records;
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
                GameInfo gameInfo = await Api.PostJsonAsync<GameInfo, GameInfo>("game-echo", new GameInfo(
                    "SE3",
                    "xxx",
                    Core.Enums.GameTag.Adventure,
                    [],
                    ["", "", ""]
                    ));

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