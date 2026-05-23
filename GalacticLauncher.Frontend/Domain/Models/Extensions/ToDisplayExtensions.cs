using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Frontend.Domain.Models.Extensions;

internal static class ToDisplayExtensions
{
    public static GameDisplay ToDisplay(this Game game, GameData? gameData = null)
    {
        return new GameDisplay
        {
            Title = game.Name,
            Description = game.Description,
            IconUrl = game.IconUrl,
        };
    }
}
