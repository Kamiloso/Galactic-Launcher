using GalacticLauncher.Frontend.Repositories;
using GalacticLauncher.Frontend.Services.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using GalacticLauncher.Core;
using GalacticLauncher.Core.Extensions;

namespace GalacticLauncher.Frontend.Services.Data;

public interface IGameDataService
{
    IEnumerable<long> GetAllGames();
    IEnumerable<long> GetLibraryGames();
    IEnumerable<long> GetFavoriteGames();

    IEnumerable<long> ObtainAllRecommendations(int limit);
    IEnumerable<long> ObtainLibraryRecommendations(int limit);
    IEnumerable<long> ObtainFavoriteRecommendations(int limit);

    // WARNING:

    // Favorites and library are not independent.
    // Adding to favorites also adds to library,
    // and removing from library also removes from favorites.

    void AddToLibrary(long gameId);
    void RemoveFromLibrary(long gameId);
    void AddToFavorite(long gameId);
    void RemoveFromFavorite(long gameId);
}

internal class GameDataService(
    IDataRepository dataRepository,
    ICacheProvider cacheProvider) : IGameDataService
{
    private List<long> _shuffledAll = [];
    private List<long> _shuffledLibrary = [];
    private List<long> _shuffledFavorites = [];

    private readonly Random _rand = new();

    public IEnumerable<long> GetAllGames()
    {
        return cacheProvider.GetAllGameIds()
            .OrderBy(id => cacheProvider.GetDisplayOf(id).Title);
    }

    public IEnumerable<long> GetLibraryGames()
    {
        List<long> allGames = [.. cacheProvider.GetAllGameIds()];
        List<long> libGames = [.. dataRepository.GetAll(Const.KEY_LIB)];

        foreach (long id in libGames.Except(allGames))
        {
            RemoveFromLibrary(id);
        }

        return dataRepository.GetAll(Const.KEY_LIB)
            .OrderBy(id => cacheProvider.GetDisplayOf(id).Title);
    }

    public IEnumerable<long> GetFavoriteGames()
    {
        List<long> allGames = [.. cacheProvider.GetAllGameIds()];
        List<long> favGames = [.. dataRepository.GetAll(Const.KEY_LIB)];

        foreach (long id in favGames.Except(allGames))
        {
            RemoveFromFavorite(id);
        }

        return dataRepository.GetAll(Const.KEY_LIB)
            .OrderBy(id => cacheProvider.GetDisplayOf(id).Title);
    }

    public IEnumerable<long> ObtainAllRecommendations(int limit)
    {
        return ObtainRecommendationsInternal(
            limit, [.. GetAllGames()], ref _shuffledAll);
    }

    public IEnumerable<long> ObtainLibraryRecommendations(int limit)
    {
        return ObtainRecommendationsInternal(
            limit, [.. GetLibraryGames()], ref _shuffledLibrary);
    }

    public IEnumerable<long> ObtainFavoriteRecommendations(int limit)
    {
        return ObtainRecommendationsInternal(
            limit, [.. GetFavoriteGames()], ref _shuffledFavorites);
    }

    private IEnumerable<long> ObtainRecommendationsInternal(int limit,
        List<long> current, ref List<long> refShuffled)
    {
        refShuffled = [.. current
            .Shuffle(_rand)
            .Limit(limit)];

        return [.. refShuffled];
    }

    public void AddToLibrary(long id)
    {
        dataRepository.Add(Const.KEY_LIB, id);
    }

    public void RemoveFromLibrary(long id)
    {
        dataRepository.Remove(Const.KEY_FAV, id);
        dataRepository.Remove(Const.KEY_LIB, id);
    }

    public void AddToFavorite(long id)
    {
        dataRepository.Add(Const.KEY_LIB, id);
        dataRepository.Add(Const.KEY_FAV, id);
    }

    public void RemoveFromFavorite(long id)
    {
        dataRepository.Remove(Const.KEY_FAV, id);
    }
}
