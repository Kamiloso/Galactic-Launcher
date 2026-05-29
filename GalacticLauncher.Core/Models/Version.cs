namespace GalacticLauncher.Core.Models;

public record Version
{
    public required long Id { get; init; }
    public required string Caption { get; init; }
    public required VersionType Type { get; init; }
    public required string Description { get; init; }
    public string? CliArgs { get; init; }
    public required bool IsPrimary { get; init; }
    public DateOnly ReleaseDate { get; init; }
    public required Platform Platform { get; init; }
    public required string DownloadUrl { get; init; }
    public required string ExecLocation { get; init; }
    public required string? Sha256Hash { get; init; }
    public required AlertLevel Alert { get; init; }
}
