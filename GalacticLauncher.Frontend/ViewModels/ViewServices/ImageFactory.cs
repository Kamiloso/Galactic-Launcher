using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.ViewModels.ImageControls;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

internal interface IImageFactory
{
    ImageViewModel CreateAndStartLoadingImage(string? imageURL);
}

internal class ImageFactory(IImageProvider imageProvider): IImageFactory
{
    public ImageViewModel CreateAndStartLoadingImage(string? url)
    {
        ImageViewModel ivm = new(imageProvider);
        _ = ivm.SetActiveLookAsync(url);

        return ivm;
    }
}
