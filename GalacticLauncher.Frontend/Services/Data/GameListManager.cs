using GalacticLauncher.Frontend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using GalacticLauncher.Core.Extensions;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Services.Cache;

namespace GalacticLauncher.Frontend.Services.Data;

public interface IGameListManager
{
    event Action OnListsChanged;

    IEnumerable<long> GetLibraryGames(string searchName = "");
    IEnumerable<long> GetFavoriteGames(string searchName = "");
    IEnumerable<long> GetNolibGames(string searchName = "");

    IEnumerable<long> ObtainLibraryRecommendations(int limit);
    IEnumerable<long> ObtainFavoriteRecommendations(int limit);
    IEnumerable<long> ObtainNolibRecommendations(int limit);

    // WARNING:

    // Favorites and library are not independent.
    // Adding to favorites also adds to library,
    // and removing from library also removes from favorites.

    bool InLibrary(long gameId);
    void AddToLibrary(long gameId);
    void RemoveFromLibrary(long gameId);
    bool InFavorite(long gameId);
    void AddToFavorite(long gameId);
    void RemoveFromFavorite(long gameId);
}

internal class GameListManager(
    IDataRepository dataRepository,
    ICacheProvider cacheProvider) : IGameListManager
{
    private const string CKEY_LIBRARY = "library";
    private const string CKEY_FAVORITE = "favorites";

    public event Action? OnListsChanged;

    private readonly Random _rand = new();

    public IEnumerable<long> GetLibraryGames(string searchName = "")
    {
        return GetFilteredGames(CKEY_LIBRARY, RemoveFromLibrary, searchName);
    }

    public IEnumerable<long> GetFavoriteGames(string searchName = "")
    {
        return GetFilteredGames(CKEY_FAVORITE, RemoveFromFavorite, searchName);
    }

    public IEnumerable<long> GetNolibGames(string searchName = "")
    {
        List<long> allGames = [.. cacheProvider.GetAllGames().Select(g => g.Id)];

        return ProcessGames(allGames.Except(GetLibraryGames()), searchName);
    }

    private IEnumerable<long> GetFilteredGames(string key, Action<long> removeAction, string searchName)
    {
        List<long> allGames = [.. cacheProvider.GetAllGames().Select(g => g.Id)];
        List<long> storedGames = [.. dataRepository.GetAll(key)];

        foreach (long id in storedGames.Except(allGames))
        {
            removeAction(id);
        }

        return ProcessGames(dataRepository.GetAll(key), searchName);
    }

    private IEnumerable<long> ProcessGames(IEnumerable<long> games, string searchName)
    {
        return games
            .Where(id => FilterGame(id, searchName))
            .OrderBy(id => cacheProvider.GetGameOf(id)?.Name ?? "");
    }

    private bool FilterGame(long id, string searchName)
    {
        Game? game = cacheProvider.GetGameOf(id);
        if (game == null) return false;

        return game.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase);
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

    public bool InLibrary(long id)
    {
        return dataRepository.GetAll(CKEY_LIBRARY).Contains(id);
    }

    public void AddToLibrary(long id)
    {
        dataRepository.Add(CKEY_LIBRARY, id);
        OnListsChanged?.Invoke();
    }

    public void RemoveFromLibrary(long id)
    {
        dataRepository.Remove(CKEY_FAVORITE, id);
        dataRepository.Remove(CKEY_LIBRARY, id);
        OnListsChanged?.Invoke();
    }

    public bool InFavorite(long id)
    {
        return dataRepository.GetAll(CKEY_FAVORITE).Contains(id);
    }

    public void AddToFavorite(long id)
    {
        dataRepository.Add(CKEY_LIBRARY, id);
        dataRepository.Add(CKEY_FAVORITE, id);
        OnListsChanged?.Invoke();
    }

    public void RemoveFromFavorite(long id)
    {
        dataRepository.Remove(CKEY_FAVORITE, id);
        OnListsChanged?.Invoke();
    }
}
