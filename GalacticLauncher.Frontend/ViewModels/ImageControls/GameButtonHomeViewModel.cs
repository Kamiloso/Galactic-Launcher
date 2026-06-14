using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.ViewModels.ImageControls;

public partial class GameButtonHomeViewModel(
    IImageProvider imageProvider,
    INavigator navigator) : GameButtonViewModel(imageProvider, navigator)
{
    public async Task SetActiveLookAsync(Game game)
    {
        await SetActiveLookAsync(game.IconUrl);
    }
}
