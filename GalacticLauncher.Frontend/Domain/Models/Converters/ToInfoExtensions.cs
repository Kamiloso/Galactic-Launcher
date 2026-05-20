using GalacticLauncher.Core.Models;
using System.Linq;

namespace GalacticLauncher.Frontend.Domain.Models.Converters;

internal static class ToInfoExtensions
{
    public static GameInfo ToInfo(this GameData gameData)
    {
        return new GameInfo
        {
            GameUnique = Normalize($"Game_{gameData.Id}"),
        };
    }

    public static ExecInfo UpgradeToExecInfo(this GameInfo gameInfo, Version version)
    {
        return new ExecInfo
        {
            GameUnique = gameInfo.GameUnique,
            VersionUnique = Normalize($"Version_{version.Id}"),
            DownloadUrl = version.DownloadUrl,
            ExecLocation = version.ExecLocation,
            Sha256Hash = version.Sha256Hash,
        };
    }

    public static (GameInfo, ExecInfo[]) ToFullInfo(this GameData gameData)
    {
        GameInfo gameInfo = gameData.ToInfo();
        ExecInfo[] execInfos = [.. gameData.Versions
            .Select(gameInfo.UpgradeToExecInfo)];

        return (gameInfo, execInfos);
    }

    private static string Normalize(string input)
    {
        return new string([.. input
            .Select(c => char.IsLetterOrDigit(c) ? c : '_')]);
    }
}
