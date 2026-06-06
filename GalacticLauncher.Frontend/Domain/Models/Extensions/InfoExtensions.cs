using GalacticLauncher.Core;
using GalacticLauncher.Core.Models;
using System.Linq;

namespace GalacticLauncher.Frontend.Domain.Models.Extensions;

internal static class InfoExtensions
{
    public static GameData RemoveIncompatiblePlatforms(this GameData gameData)
    {
        return gameData with
        {
            Versions = [.. gameData.Versions
                .Where(ver => ver.Platform == Utils.CurrentPlatform)]
        };
    }
}
