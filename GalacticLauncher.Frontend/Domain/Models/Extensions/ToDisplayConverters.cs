using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Frontend.Domain.Models.Extensions;

internal static class ToDisplayConverters
{
    public static GameDisplay ToDisplay(this Game game)
    {
        return new GameDisplay
        {
            Id = game.Id,
            Title = game.Name,
            Description = game.Description,
            IconUrl = game.IconUrl,
        };
    }

    public static GameDisplay ToDisplay(this GameData gameData)
    {
        return new GameDisplay
        {
            Id = gameData.Id,
            Title = gameData.Name,
            Description = gameData.Description,
            IconUrl = gameData.IconUrl,
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
