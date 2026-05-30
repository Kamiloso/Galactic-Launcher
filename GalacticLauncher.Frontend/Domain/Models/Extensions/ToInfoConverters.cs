using GalacticLauncher.Core.Models;
using System.Linq;

namespace GalacticLauncher.Frontend.Domain.Models.Extensions;

internal static class ToInfoConverters
{
    public static ExecInfo ToExecInfo(this GameData gameData, Version version)
    {
        return new ExecInfo
        {
            GameUnique = Normalize($"Game_{gameData.Id}"),
            VersionUnique = Normalize($"Version_{version.Id}"),
            DownloadUrl = version.DownloadUrl,
            ExecLocation = version.ExecLocation,
            Sha256Hash = version.Sha256Hash,
            CliArgs = version.CliArgs ?? string.Empty,
        };

        static string Normalize(string input)
        {
            return new string([.. input
                .Select(c => char.IsLetterOrDigit(c) ? c : '_')]);
        }
    }
}
