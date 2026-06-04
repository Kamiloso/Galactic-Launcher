namespace GalacticLauncher.Backend.Domain.Models;

// Dynamically constructed SQL object based on 'games' table
// with additional fields from other tables
public record GamePlusEntity : GameEntity
{
    public required string? IconUrl { get; init; }
    public required string? TagIdList { get; init; }
}
