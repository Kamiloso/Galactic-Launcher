using GalacticLauncher.Core.Models;
using System.Linq;

namespace GalacticLauncher.Frontend.Domain.Models.Extensions;

internal static class ToInfoConverters
{
    public static GameInfo ToGameInfo(this Game game)
    {
        return new GameInfo
        {
            GameId = game.Id,
            GameName = game.Name,
            GameUnique = Normalize($"Game_{game.Id}"),
        };
    }

    public static ExecInfo ToExecInfo(this Game game, Version version)
    {
        return game
            .ToGameInfo()
            .UpgradeToExecInfo(version);
    }

    public static ExecInfo UpgradeToExecInfo(this GameInfo gameInfo, Version version)
    {
        return new ExecInfo
        {
            GameId = gameInfo.GameId,
            GameName = gameInfo.GameName,
            GameUnique = gameInfo.GameUnique,
            VersionId = version.Id,
            VersionName = version.Caption,
            VersionUnique = Normalize($"Version_{version.Id}"),
            DownloadUrl = version.DownloadUrl,
            ExecLocation = version.ExecLocation,
            Sha256Hash = version.Sha256Hash,
            CliArgs = version.CliArgs,
        };
    }

    private static string Normalize(string input)
    {
        return new string([.. input
            .Select(c => char.IsLetterOrDigit(c) ? c : '_')]);
    }
}
