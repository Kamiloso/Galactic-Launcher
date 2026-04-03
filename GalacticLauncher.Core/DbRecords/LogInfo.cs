using GalacticLauncher.Core.Enums;

namespace GalacticLauncher.Core.DbRecords;

public record LogInfo
{
    public required long Id { get; init; } // PK
    public required LogType Type { get; init; } // types: Download, Login etc.
    public DateTime Time { get; init; } = DateTime.UtcNow; // when it was made
    public required long IdUser { get; init; } // FK
    public long? IdExec { get; init; } // FK (Nullable)
}