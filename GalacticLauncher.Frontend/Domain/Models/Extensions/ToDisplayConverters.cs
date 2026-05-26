using GalacticLauncher.Core.Models;
using System.Linq;

namespace GalacticLauncher.Frontend.Domain.Models.Extensions;

internal static class ToDisplayConverters
{
    public static GameDisplay ToDisplay(this Game game, GameData? gameData = null)
    {
        return new GameDisplay
        {
            Id = game.Id,
            Title = game.Name,
            Author = game.Author,
            Description = game.Description,
            IconUrl = gameData?.Versions.Count().ToString(),
        };
    }

    public static VersionDisplay ToDisplay(this Version version)
    {
        return new VersionDisplay
        {
            Id = version.Id,
            Caption = version.Caption,
            Description = version.Description,
            IsPrimary = version.IsPrimary,
            ReleaseDate = version.ReleaseDate,
        };
    }
}
