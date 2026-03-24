using GalacticLauncher.Core.Enums;
using System;

namespace GalacticLauncher.Core.DbRecords;

public record VersionInfo(
    ulong Id, // PK
    string VersionText, // example: "2.3b"
    string Description, // changelog for example
    bool IsPrimary, // the main game version
    DateTime ReleaseDate, // upload date
    VersionType VersionType, // Alpha, Beta, Release etc.
    ulong IdGame // FK
    )
{
    public bool IsRelease() =>
        VersionType == VersionType.Release;
}
