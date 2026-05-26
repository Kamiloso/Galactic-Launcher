using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.UserData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GalacticLauncher.Frontend.Services.Files;

public interface IGameSelectionService
{
    IEnumerable<long> GetRecommendedGames(int count);
    IEnumerable<long> GetFavoriteGames(int count);
    long? GetLastAddedLibraryGame();
}
internal class GameSelectionService(
    IUserDataService userDataService,
    ICacheProvider cacheProvider) : IGameSelectionService
{
    private readonly Random _random = new();

    public IEnumerable<long> GetRecommendedGames(int count)
    {
        var allGames = cacheProvider.AllGameIds();

        var libraryGames = userDataService.GetLibraryIds();

        return allGames
            .Except(libraryGames)
            .OrderBy(_ => _random.Next())
            .Take(count);
    }

    public IEnumerable<long> GetFavoriteGames(int count)
    {
        return userDataService.GetFavoriteIds()
            .OrderBy(_ => _random.Next())
            .Take(count);
    }

    public long? GetLastAddedLibraryGame()
    {
        var library = userDataService.GetLibraryIds();
        return library.LastOrDefault();
    }
}
