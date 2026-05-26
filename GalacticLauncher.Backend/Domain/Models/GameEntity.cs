namespace GalacticLauncher.Backend.Domain.Models;

// Represents the 'games' table
public record GameEntity
{
    public long Id { get; init; } = 0; // auto_increment
    public required string Name { get; init; }
    public required string Author { get; init; }
    public required string Description { get; init; }
}
