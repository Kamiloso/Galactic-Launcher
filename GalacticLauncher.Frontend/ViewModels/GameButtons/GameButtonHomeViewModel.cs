using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.GameButtons;

internal partial class GameButtonHomeViewModel(
    IImageProvider imageProvider,
    INavigator navigator) : GameButtonViewModel(imageProvider, navigator)
{
    // All functionality is implemented in the base class,
    // but everything can be extended in the future if needed.
}
