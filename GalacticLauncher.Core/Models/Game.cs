namespace GalacticLauncher.Core.Models;

public record Game
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string? IconUrl { get; init; }
}
