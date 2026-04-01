using GalacticLauncher.Core.Enums;
using System;

namespace GalacticLauncher.Core.DbRecords;

public record VersionInfo
{
    public required ulong Id { get; init; } // PK
    public required string VersionText { get; init; } // example: "2.3b"
    public required string Description { get; init; } // changelog for example
    public required bool IsPrimary { get; init; } // the main game version
    public required DateTime ReleaseDate { get; init; } // upload date
    public required VersionType VersionType { get; init; } // Alpha, Beta, Release etc.
    public required ulong IdGame { get; init; } // FK
    public GameInfo? Game { get; init; }
    
    public ICollection<ExecInfo> Executables { get; init; } = [];

    public bool IsRelease() => 
        VersionType == VersionType.Release;
}