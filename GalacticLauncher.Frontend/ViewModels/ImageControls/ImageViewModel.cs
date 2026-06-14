using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Services;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.ViewModels.ImageControls;

internal partial class ImageViewModel(IImageProvider imageProvider) : ObservableObject
{
    protected const string EMPTY_STATUS = "";
    protected const string LOADING_IMAGE = "LOADING IMAGE...";
    protected const string IMAGE_NOT_FOUND = "IMAGE NOT FOUND";

    [ObservableProperty]
    private string _statusMessage = EMPTY_STATUS;

    [ObservableProperty]
    private Bitmap? _image;

    public async Task SetActiveLookAsync(string? url)
    {
        if (url == null)
        {
            StatusMessage = IMAGE_NOT_FOUND;
            return;
        }

        StatusMessage = LOADING_IMAGE;

        try
        {
            string filePath = await imageProvider.GetImagePathAsync(url);

            Image = new Bitmap(filePath);
            StatusMessage = EMPTY_STATUS;
        }
        catch (DownloadException)
        {
            StatusMessage = IMAGE_NOT_FOUND;
        }
    }
}
