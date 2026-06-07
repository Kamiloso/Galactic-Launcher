using GalacticLauncher.Core.Dto;

namespace GalacticLauncher.Frontend.Domain.Models.Extensions;

internal static class TelemetryConverters
{
    public static PlayGame ToPlayGame(this ExecInfo execInfo)
    {
        return new PlayGame
        {
            GameId = execInfo.GameId,
            GameName = execInfo.GameName,
            VersionId = execInfo.VersionId,
            VersionName = execInfo.VersionName,
        };
    }
}
