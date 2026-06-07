using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.ViewModels.ImageLoad;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

internal interface IImageFactory
{
    ImageViewModel CreateAndStartLoadingImage(string? imageURL);
}

internal class ImageFactory(IImageProvider imageProvider): IImageFactory
{
    public ImageViewModel CreateAndStartLoadingImage(string? imageUrl)
    {
        ImageViewModel ivm = new(imageProvider) { ImageUrl = imageUrl };
        _ = ivm.SetActiveLookAsync();

        return ivm;
    }
}
