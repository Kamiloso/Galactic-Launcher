namespace GalacticLauncher.Core.Models;

public record GameData
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required IEnumerable<Version> Versions { get; init; }
    public required IEnumerable<Image> Images { get; init; }
    public required IEnumerable<Tag> Tags { get; init; }
}
