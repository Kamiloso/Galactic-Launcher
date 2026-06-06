namespace GalacticLauncher.Frontend.Domain.Models;

/// <summary>
/// Represents exec data that is necessary while working with game executables.
/// </summary>

public record GameInfo
{
    public required string GameUnique { get; init; }
}

public record ExecInfo : GameInfo
{
    public required string VersionUnique { get; init; }
    public required string CliArgs { get; init; }
    public required string DownloadUrl { get; init; }
    public required string ExecLocation { get; init; }
    public required string? Sha256Hash { get; init; }
}
