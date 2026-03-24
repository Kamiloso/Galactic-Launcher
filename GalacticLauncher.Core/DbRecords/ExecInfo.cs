using GalacticLauncher.Core.Enums;

namespace GalacticLauncher.Core.DbRecords;

public record ExecInfo
{
    public ulong Id { get; init; } // PK
    public string DownloadUrl { get; init; } = string.Empty; // where is it in web?
    public string ExecLocation { get; init; } = string.Empty; // where is it in folder?
    public string FileHashSHA256 { get; init; } = string.Empty; // for integrity & auto-patching
    public PlatformType Platform { get; init; } // Windows / Linux / macOS etc.
    public AlertType Alert { get; init; } // how safe is it to run?
    public ulong IdVersion { get; init; } // FK
}