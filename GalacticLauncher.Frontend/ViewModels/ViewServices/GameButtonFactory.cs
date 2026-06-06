using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.ViewModels.GameButtons;
using GalacticLauncher.Frontend.ViewModels.ImageLoad;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

internal interface IGameButtonFactory
{
    GameButtonHomeViewModel CreateAndStartLoading(long gameId);
    GameButtonLibraryViewModel CreateAndStartLoadingLibrary(long gameId);
}

internal class GameButtonFactory(
    ICacheProvider cacheProvider,
    IImageProvider imageProvider,
    IGameListManager gameListManager,
    INavigator navigator) : IGameButtonFactory
{
    public GameButtonHomeViewModel CreateAndStartLoading(long gameId)
    {
        var gbvm = new GameButtonHomeViewModel(imageProvider, navigator) { Id = gameId };

        Game? game = cacheProvider.GetGameOf(gameId);

        _ = gbvm.SetActiveLookAsync(game);

        return gbvm;
    }

    public GameButtonLibraryViewModel CreateAndStartLoadingLibrary(long gameId)
    {
        var gbvm = new GameButtonLibraryViewModel(
            imageProvider, gameListManager, cacheProvider, navigator) { Id = gameId };

        Game? game = cacheProvider.GetGameOf(gameId);

        _ = gbvm.SetActiveLookAsync(game);

        return gbvm;
    }
}
