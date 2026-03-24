using GalacticLauncher.Core.Enums;
using System;

namespace GalacticLauncher.Core.DbRecords;

public record VersionInfo
{
    public ulong Id { get; init; } // PK
    public string VersionText { get; init; } = string.Empty; // example: "2.3b"
    public string Description { get; init; } = string.Empty; // changelog for example
    public bool IsPrimary { get; init; } // the main game version
    public DateTime ReleaseDate { get; init; } // upload date
    public VersionType VersionType { get; init; } // Alpha, Beta, Release etc.
    public ulong IdGame { get; init; } // FK

    public bool IsRelease() => 
        VersionType == VersionType.Release;
}