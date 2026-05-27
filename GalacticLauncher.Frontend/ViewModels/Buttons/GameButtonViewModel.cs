using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Services.Files;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.Buttons
{
    internal partial class GameButtonViewModel: ObservableObject
    {
        private const string NO_IMAGE = "NO IMAGE FOUND";
        private const string NO_GAME = "NO GAME FOUND";

        [ObservableProperty]
        private bool _isGameValid;
        public long GameId { get; }

        private readonly IImageService _imageService;
        private readonly INavigator _navigator;

        [ObservableProperty]
        private Bitmap? _icon;

        [ObservableProperty]
        private string _statusMessage = NO_IMAGE;

        public GameButtonViewModel(
            long gameId, 
            IImageService imageService, 
            INavigator navigator
        )
        {
            GameId = gameId;
            _imageService = imageService;
            _navigator = navigator;

            ShowGameCommand.NotifyCanExecuteChanged();
        }

        public async Task LoadAsync(string url)
        {

            if (GameId == -1)
            {
                StatusMessage = NO_GAME;
                IsGameValid = false;
                return;
            }
            IsGameValid = true;

            try
            {
                string file = await _imageService.DownloadImageAsync(GameId, url);
                if (File.Exists(file))
                {
                    var bitmap = new Bitmap(file);
                    Icon = bitmap;
                }
            }
            catch (DownloadException)
            { 
                StatusMessage = NO_IMAGE;
            }
        }


        [RelayCommand]
        public void ShowGame() => _navigator.NavigateTo<GameViewModel>(GameId);
    }
}
