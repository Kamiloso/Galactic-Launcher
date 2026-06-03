namespace GalacticLauncher.Backend.Domain.Models;

// Represents the 'images' table
public record ImageEntity
{
    public required long Id { get; init; }
    public required string DownloadUrl { get; init; }
    public required string Type { get; init; }
    public required int SortIndex { get; init; }
    public required long IdGame { get; init; }
}
