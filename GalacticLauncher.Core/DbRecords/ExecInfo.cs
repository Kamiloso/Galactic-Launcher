using GalacticLauncher.Core.Enums;

namespace GalacticLauncher.Core.DbRecords;

public record ExecInfo
{
    public required long Id { get; init; } // PK
    public required string DownloadUrl { get; init; } // where is it in web?
    public required string ExecLocation { get; init; } // where is it in folder?
    public required string FileHashSHA256 { get; init; } // for integrity & auto-patching
    public required PlatformType Platform { get; init; } // Windows / Linux / macOS etc.
    public required AlertType Alert { get; init; } // how safe is it to run?
    public required long IdVersion { get; init; } // FK
}