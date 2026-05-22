using System.Collections.Generic;
using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Frontend.Repositories;

internal class DataRepository
{
    private readonly HashSet<long> _library = [];
    private readonly HashSet<long> _favorite = [];

    public void AddToLibrary(long id) => _library.Add(id);
    public bool RemoveFromLibrary(long id) => _library.Remove(id);
    public void ClearLibrary() => _library.Clear();

    public void AddToFavorite(long id) => _favorite.Add(id);
    public bool RemoveFromFavority(long id) => _favorite.Remove(id);
    public void ClearFavorite() => _favorite.Clear();

    private readonly Dictionary<long, Game> _dictGame = [];
    private readonly Dictionary<long, GameData> _dictGameData = [];

    public void ClearCache()
    {
        _dictGame.Clear();
        _dictGameData.Clear();
    }
}
