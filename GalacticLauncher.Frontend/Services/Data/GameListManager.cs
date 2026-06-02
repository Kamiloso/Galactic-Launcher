using GalacticLauncher.Frontend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using GalacticLauncher.Core;
using GalacticLauncher.Core.Extensions;
using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Frontend.Services.Data;

public interface IGameListManager
{
    IEnumerable<long> GetLibraryGames(string? searchName = null);
    IEnumerable<long> GetFavoriteGames(string? searchName = null);
    IEnumerable<long> GetNolibGames(string? searchName = null);

    IEnumerable<long> ObtainLibraryRecommendations(int limit);
    IEnumerable<long> ObtainFavoriteRecommendations(int limit);
    IEnumerable<long> ObtainNolibRecommendations(int limit);

    // WARNING:

    // Favorites and library are not independent.
    // Adding to favorites also adds to library,
    // and removing from library also removes from favorites.

    void AddToLibrary(long gameId);
    void RemoveFromLibrary(long gameId);
    void AddToFavorite(long gameId);
    void RemoveFromFavorite(long gameId);
}

internal class GameListManager(
    IDataRepository dataRepository,
    ICacheProvider cacheProvider) : IGameListManager
{
    private readonly Random _rand = new();

    public IEnumerable<long> GetLibraryGames(string? searchName = null)
    {
        List<long> allGames = [.. cacheProvider.GetAllGames().Select(g => g.Id)];
        List<long> libGames = [.. dataRepository.GetAll(Const.KEY_LIB)];

        foreach (long id in libGames.Except(allGames))
        {
            RemoveFromLibrary(id);
        }

        return dataRepository.GetAll(Const.KEY_LIB)
            .Where(id => FilterGame(id, searchName))
            .OrderBy(id => cacheProvider.GetGameOf(id)?.Name ?? "");
    }

    public IEnumerable<long> GetFavoriteGames(string? searchName = null)
    {
        List<long> allGames = [.. cacheProvider.GetAllGames().Select(g => g.Id)];
        List<long> favGames = [.. dataRepository.GetAll(Const.KEY_FAV)];

        foreach (long id in favGames.Except(allGames))
        {
            RemoveFromFavorite(id);
        }

        return dataRepository.GetAll(Const.KEY_FAV)
            .Where(id => FilterGame(id, searchName))
            .OrderBy(id => cacheProvider.GetGameOf(id)?.Name ?? "");
    }

    public IEnumerable<long> GetNolibGames(string? searchName = null)
    {
        List<long> allGames = [.. cacheProvider.GetAllGames().Select(g => g.Id)];

        List<long> libGames = [.. GetLibraryGames()];
        List<long> nolibGames = [.. allGames.Except(libGames)];

        return nolibGames
            .Where(id => FilterGame(id, searchName))
            .OrderBy(id => cacheProvider.GetGameOf(id)?.Name ?? "");
    }

    private bool FilterGame(long id, string? searchName)
    {
        Game? game = cacheProvider.GetGameOf(id);
        if (game == null) return false;

        if (!string.IsNullOrWhiteSpace(searchName))
        {
            return game.Name?.Contains(searchName, StringComparison.OrdinalIgnoreCase) ?? false;
        }
        return true;
    }

    public IEnumerable<long> ObtainLibraryRecommendations(int limit)
    {
        return ObtainRecommendationsInternal(
            limit, [.. GetLibraryGames()]);
    }

    public IEnumerable<long> ObtainFavoriteRecommendations(int limit)
    {
        return ObtainRecommendationsInternal(
            limit, [.. GetFavoriteGames()]);
    }

    public IEnumerable<long> ObtainNolibRecommendations(int limit)
    {
        return ObtainRecommendationsInternal(
            limit, [.. GetNolibGames()]);
    }

    private IEnumerable<long> ObtainRecommendationsInternal(int limit, List<long> current)
    {
        return [.. current
            .Shuffle(_rand)
            .Limit(limit)];
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
