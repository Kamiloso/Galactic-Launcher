using GalacticLauncher.Core;

namespace GalacticLauncher.Backend.Domain.Models;

// Represents the 'images' table
public record ImageEntity
{
    public long Id { get; init; } = 0; // auto_increment
    public required string DownloadUrl { get; init; }
    public required ImageType Type { get; init; }
    public int SortIndex { get; init; } = 0; 
    public required long IdGame { get; init; }
}
