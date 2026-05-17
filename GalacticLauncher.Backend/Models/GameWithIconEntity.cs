namespace GalacticLauncher.Backend.Models;

// Dynamically constructed SQL object based on 'games' table
// with additional field for icon url from 'images' table (if exists)
public record GameWithIconEntity : GameEntity
{
    public required string? IconUrl { get; init; }
}
