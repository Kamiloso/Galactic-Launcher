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
    internal partial class GameButtonViewModel(long gameId, IImageService imageService, INavigator navigator) : ObservableObject
    {
        public long GameId { get; } = gameId;
        private readonly IImageService _imageService = imageService;
        private readonly INavigator _navigator = navigator;

        [ObservableProperty]
        private Bitmap? _icon;

        public async Task LoadAsync(string url)
        {
            try
            {
                string file = await _imageService.DownloadImageAsync(GameId, url);
                if (File.Exists(file))
                {
                    Icon = new Bitmap(file);
                }
            }
            catch (DownloadException) { }
        }

        [RelayCommand]
        public void ShowGame() => _navigator.NavigateTo<GameViewModel>(GameId);
    }
}
