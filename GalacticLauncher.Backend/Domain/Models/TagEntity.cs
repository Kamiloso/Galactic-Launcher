namespace GalacticLauncher.Backend.Domain.Models;

// Represents the 'tags' table
public record TagEntity
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}
