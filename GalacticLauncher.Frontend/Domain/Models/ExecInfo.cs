namespace GalacticLauncher.Frontend.Domain.Models;

/// <summary>
/// Represents exec data that is necessary while working with game executables.
/// </summary>
public record ExecInfo : GameInfo
{
    public required long VersionId { get; init; }
    public required string VersionName { get; init; }
    public required string VersionUnique { get; init; }
    public required string CliArgs { get; init; }
    public required string DownloadUrl { get; init; }
    public required string ExecLocation { get; init; }
    public required string? Sha256Hash { get; init; }

    public string GetIdentity()
    {
        return $"{GameUnique}::{VersionUnique}";
    }

    public string GetFullName()
    {
        return $"{GameName} {VersionName}";
    }
}
