using GalacticLauncher.Core.Enums;
using System;

namespace GalacticLauncher.Core.DbRecords;

public record LogInfo
{
    public ulong Id { get; init; } // PK
    public LogType Type { get; init; } // types: Download, Login etc.
    public DateTime Time { get; init; } // when it was made
    public ulong IdUser { get; init; } // FK
    public ulong? IdExec { get; init; } // FK?
}