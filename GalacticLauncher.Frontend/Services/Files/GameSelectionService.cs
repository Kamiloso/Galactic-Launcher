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

    private HashSet<long> _randomRec =[];
    private HashSet<long> _randomFav = [];


    public IEnumerable<long> GetRecommendedGames(int count)
    {
        var allGames = cacheProvider.AllGameIds();
        var libraryGames = userDataService.GetLibraryIds();

        var toRemove = _randomRec.Where(id => !allGames.Contains(id)).ToList();

        foreach (var id in toRemove)
        {
            _randomRec.Remove(id);
        }

        if (_randomRec.Count < count)
        {
            var needed = count - _randomRec.Count;
            var availablePool = allGames.Except(libraryGames).Except(_randomRec).ToList();

            var newGames = availablePool
                .OrderBy(_ => _random.Next())
                .Take(needed);

            foreach (var game in newGames)
            {
                _randomRec.Add(game);
            }
        }

        return _randomRec;
    }

    public IEnumerable<long> GetFavoriteGames(int count)
    {
        var currentFavorites = userDataService.GetFavoriteIds();

        var allfavorite = currentFavorites.ToHashSet();
        var toRemove = _randomFav.Where(id => !allfavorite.Contains(id)).ToList();

        foreach (var id in toRemove)
        {
            _randomFav.Remove(id);
        }

        if (_randomFav.Count < count)
        {
            var needed = count - _randomFav.Count;

            var availablePool = currentFavorites.Except(_randomFav).ToList();

            var newGames = availablePool
                .OrderBy(_ => _random.Next())
                .Take(needed);

            foreach (var game in newGames)
            {
                _randomFav.Add(game);
            }
        }

        return _randomFav;
    }

    public long? GetLastAddedLibraryGame()
    {
        var library = userDataService.GetLibraryIds();
        return library.LastOrDefault();
    }
}
