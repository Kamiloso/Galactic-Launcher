using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.ViewModels.GameButtons;
using GalacticLauncher.Frontend.ViewModels.ImageLoad;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices
{
    internal interface IImageFactory
    {
        ImageViewModel CreateAndStartLoadingImage(string? imageURL);
    }
    internal class ImageFactory(
        IImageProvider imageProvider
        ): IImageFactory
    {
        public ImageViewModel CreateAndStartLoadingImage(string? ImageURL)
        {
            ImageViewModel ivm = new ImageViewModel(imageProvider, ImageURL);
            _ = ivm.SetActiveLookAsync();

            return ivm;
        }
    }
}
