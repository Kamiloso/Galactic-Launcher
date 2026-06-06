using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Services;

namespace GalacticLauncher.Frontend.ViewModels.ImageLoad
{
    internal partial class ImageViewModel(
        IImageProvider imageProvider,
        string? imageURL
        ): ObservableObject
    {
        protected const string EMPTY_STATUS = "";
        protected const string LOADING_IMAGE = "LOADING IMAGE...";
        protected const string IMAGE_NOT_FOUND = "IMAGE NOT FOUND";

        [ObservableProperty]
        private string _statusMessage = EMPTY_STATUS;

        [ObservableProperty]
        private Bitmap? _image;

        public virtual async Task SetActiveLookAsync()
        {
            if (imageURL == null)
            {
                StatusMessage = IMAGE_NOT_FOUND;
                return;
            }

            StatusMessage = LOADING_IMAGE;

            try
            {
                string filePath = await imageProvider.GetImagePathAsync(imageURL);

                Image = new Bitmap(filePath);
                StatusMessage = EMPTY_STATUS;
            }
            catch (DownloadException)
            {
                StatusMessage = IMAGE_NOT_FOUND;
            }
        }
    }
}
