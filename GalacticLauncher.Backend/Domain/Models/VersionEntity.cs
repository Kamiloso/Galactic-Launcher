using GalacticLauncher.Core;

namespace GalacticLauncher.Backend.Domain.Models;

// Represents the 'versions' table
public record VersionEntity
{
    public long Id { get; init; } = 0; // auto_increment
    public required string Caption { get; init; }
    public required VersionType Type { get; init; }
    public required string Description { get; init; }
    public string? CliArgs { get; init; }
    public required bool IsPrimary { get; init; }
    public DateOnly ReleaseDate { get; init; } = DateOnly.FromDateTime(DateTime.Now);
    public required Platform Platform { get; init; }
    public required string DownloadUrl { get; init; }
    public required string ExecLocation { get; init; }
    public required string? Sha256Hash { get; init; }
    public required AlertLevel Alert { get; init; }
    public required long IdGame { get; init; }
}
