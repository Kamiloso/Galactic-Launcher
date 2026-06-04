namespace GalacticLauncher.Core.Models;

public record GameRaw
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Author { get; init; }
    public required string Description { get; init; }
}
