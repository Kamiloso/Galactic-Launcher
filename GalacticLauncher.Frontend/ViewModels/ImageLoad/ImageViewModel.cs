using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Services;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.ViewModels.ImageLoad;

internal partial class ImageViewModel(IImageProvider imageProvider): ObservableObject
{
    private const string EMPTY_STATUS = "";
    private const string LOADING_IMAGE = "LOADING IMAGE...";
    private const string IMAGE_NOT_FOUND = "IMAGE NOT FOUND";

    [ObservableProperty]
    private string _statusMessage = EMPTY_STATUS;

    [ObservableProperty]
    private Bitmap? _image;

    public required string? ImageUrl { get; init; }

    public virtual async Task SetActiveLookAsync()
    {
        if (ImageUrl == null)
        {
            StatusMessage = IMAGE_NOT_FOUND;
            return;
        }

        StatusMessage = LOADING_IMAGE;

        try
        {
            string filePath = await imageProvider.GetImagePathAsync(ImageUrl);

            Image = new Bitmap(filePath);
            StatusMessage = EMPTY_STATUS;
        }
        catch (DownloadException)
        {
            StatusMessage = IMAGE_NOT_FOUND;
        }
    }
}
