using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.Services.Images;
using GalacticLauncher.Frontend.ViewModels.Controls;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

interface ILibraryGameButtonFactory
{
    LibraryGameButtonViewModel CreateEmpty();
    LibraryGameButtonViewModel CreateAndStartLoading(long gameId);
}
internal class LibraryGameButtonFactory(
    ICacheProvider cacheProvider,
    IImageProvider imageProvider,
    IGameListManager gameListManager,
    INavigator navigator) : ILibraryGameButtonFactory
{
    public LibraryGameButtonViewModel CreateEmpty()
    {
        var gbvm = new LibraryGameButtonViewModel(imageProvider, gameListManager,navigator) { Id = null };

        gbvm.SetInactiveLook();

        return gbvm;
    }

    public LibraryGameButtonViewModel CreateAndStartLoading(long gameId)
    {
        var gbvm = new LibraryGameButtonViewModel(imageProvider, gameListManager,navigator) { Id = gameId };

        var gameData = cacheProvider.GetGameOf(gameId);
        if (gameData != null)
        {
            gbvm.GameTitle = gameData.Name ?? "";
            gbvm.GameAuthor = $"AUTHOR: {gameData.Author?.ToUpper() ?? "UNKNOWN"}";
            gbvm.GameDescription = gameData.Description ?? "";

            _ = gbvm.SetActiveLookAsync(gameData.IconUrl);
        }

        return gbvm;
    }
}
