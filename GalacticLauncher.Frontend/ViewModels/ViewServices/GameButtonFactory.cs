using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.Services.Images;
using GalacticLauncher.Frontend.ViewModels.Controls;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

internal interface IGameButtonFactory
{
    GameButtonViewModel CreateEmpty();
    GameButtonViewModel CreateAndStartLoading(long gameId);
}

internal class GameButtonFactory(
    ICacheProvider cacheProvider,
    IImageProvider imageProvider,
    INavigator navigator) : IGameButtonFactory
{
    public GameButtonViewModel CreateEmpty()
    {
        var gbvm = new GameButtonViewModel(imageProvider, navigator) { Id = null };

        gbvm.SetInactiveLook();

        return gbvm;
    }

    public GameButtonViewModel CreateAndStartLoading(long gameId)
    {
        var gbvm = new GameButtonViewModel(imageProvider, navigator) { Id = gameId };

        string? url = cacheProvider.GetGameOf(gameId)?.IconUrl;

        _ = gbvm.SetActiveLookAsync(url);

        return gbvm;
    }
}
