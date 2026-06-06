using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.ViewModels.ImageLoad;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

internal interface IImageFactory
{
    ImageViewModel CreateAndStartLoadingImage(string? imageURL);
}

internal class ImageFactory(IImageProvider imageProvider): IImageFactory
{
    public ImageViewModel CreateAndStartLoadingImage(string? imageURL)
    {
        ImageViewModel ivm = new(imageProvider) { ImageUrl = imageURL };
        _ = ivm.SetActiveLookAsync();

        return ivm;
    }
}
