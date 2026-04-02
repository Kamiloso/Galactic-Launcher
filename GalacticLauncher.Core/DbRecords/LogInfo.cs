using GalacticLauncher.Core.Enums;
using System;

namespace GalacticLauncher.Core.DbRecords;

public record LogInfo
{
    public required ulong Id { get; init; } // PK
    public required LogType Type { get; init; } // types: Download, Login etc.
    public DateTime Time { get; init; } = DateTime.UtcNow; // when it was made
    public required ulong IdUser { get; init; } // FK
    public ulong? IdExec { get; init; } // FK (Nullable)
}