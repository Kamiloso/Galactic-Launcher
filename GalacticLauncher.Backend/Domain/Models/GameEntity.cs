namespace GalacticLauncher.Backend.Domain.Models;

// Represents the 'games' table
public record GameEntity
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Author { get; init; }
    public required string Description { get; init; }
}
