namespace GalacticLauncher.Frontend.Domain.Models;

/// <summary>
/// Represents exec data that is necessary while working with game executables.
/// </summary>
public record ExecInfo
{
    public required string GameUnique { get; init; }
    public required string VersionUnique { get; init; }
    public required string CliArgs { get; init; }
    public required string DownloadUrl { get; init; }
    public required string ExecLocation { get; init; }
    public required string? Sha256Hash { get; init; }
}
