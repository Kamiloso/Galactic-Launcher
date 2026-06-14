using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.ViewModels.ImageLoad;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

public interface IImageFactory
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
