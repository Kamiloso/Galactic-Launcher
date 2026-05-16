namespace GalacticLauncher.Core.DbModels;

// Represents the 'games' table
public record Game
{
    public long Id { get; init; } = 0; // auto_increment
    public required string Name { get; init; }
    public required string Description { get; init; }
}
