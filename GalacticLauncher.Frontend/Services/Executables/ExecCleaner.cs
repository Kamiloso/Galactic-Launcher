using GalacticLauncher.Core;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using GalacticLauncher.Frontend.Services.Cache;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GalacticLauncher.Frontend.Services.Executables;

public interface IExecCleaner { }

internal class ExecCleaner : IExecCleaner
{
    private readonly ICacheProvider _cacheProvider;
    private readonly IExecPathSystem _execPathSystem;

    public ExecCleaner(
        ICacheProvider cacheProvider,
        ICacheRefresher cacheRefresher,
        IExecPathSystem execPathSystem)
    {
        _cacheProvider = cacheProvider;
        _execPathSystem = execPathSystem;

        cacheRefresher.OnBaseRefresh += CleanAllGames;
        cacheRefresher.OnRefreshGameData += CleanGameVersions;
    }

    private void CleanAllGames()
    {
        List<Game> games = [..
            _cacheProvider.GetAllGames()
        ];

        foreach (string dir in Directory.EnumerateDirectories(Utils.RootPath, "Game_*"))
        {
            if (!games.Any(g =>
            {
                return Utils.ArePathsEqual(
                    dir, _execPathSystem.PrepareGamePath(g.ToGameInfo(), false));
            }))
            {
                try { Directory.Delete(dir, true); } catch { }
            }
        }
    }

    private void CleanGameVersions(long id)
    {
        Game? game = _cacheProvider.GetGameOf(id);
        if (game is null) return;

        List<Version> versions = [..
            _cacheProvider.GetVersionsOf(id)
        ];

        string gameRootPath = _execPathSystem.PrepareGamePath(game.ToGameInfo(), false);
        if (!Directory.Exists(gameRootPath)) return;

        foreach (string dir in Directory.EnumerateDirectories(gameRootPath, "Version_*"))
        {
            if (!versions.Any(v =>
            {
                return Utils.ArePathsEqual(
                    dir, _execPathSystem.PrepareExecPath(game.ToExecInfo(v), false));
            }))
            {
                try { Directory.Delete(dir, true); } catch { }
            }
        }
    }
}
