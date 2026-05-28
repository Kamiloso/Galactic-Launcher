namespace GalacticLauncher.Core.Models;

public record Image
{
    public required string DownloadUrl { get; init; }
    public required ImageType Type { get; init; }
    public required int SortIndex { get; init; }
}
