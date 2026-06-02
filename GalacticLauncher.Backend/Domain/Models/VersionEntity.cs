using GalacticLauncher.Core;

namespace GalacticLauncher.Backend.Domain.Models;

// Represents the 'versions' table
public record VersionEntity
{
    public required long Id { get; init; }
    public required string Caption { get; init; }
    public required VersionType Type { get; init; }
    public required string Description { get; init; }
    public required string CliArgs { get; init; }
    public required bool IsPrimary { get; init; }
    public required DateOnly ReleaseDate { get; init; }
    public required Platform Platform { get; init; }
    public required string DownloadUrl { get; init; }
    public required string ExecLocation { get; init; }
    public required string? Sha256Hash { get; init; }
    public required AlertLevel Alert { get; init; }
    public required long IdGame { get; init; }
}
