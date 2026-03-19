using GalacticLauncher.Core.Enums;
using System;

namespace GalacticLauncher.Core.Records;

public record GameInfo(
    string Name, // name of the game
    string Description, // why you should play etc.
    GameTag Tags, // game types
    VersionInfo[] Versions, // list of all versions
    string[] ImgUrls // game images
    )
{
    public VersionInfo? LatestVersion() => Versions.Length > 0
        ? Versions[0] : null;

    public DateTime? ReleaseDate()
    {
        if (Versions.Length == 0)
            return null;

        return Versions
            .Where(v => v.IsRelease())
            .Min(v => v.ReleaseDate);
    }
}
