namespace GalacticLauncher.Backend.Models;

// Dynamically constructed SQL object
public record GameWithIconEntity : GameEntity
{
    public required string? IconUrl { get; init; }
}
