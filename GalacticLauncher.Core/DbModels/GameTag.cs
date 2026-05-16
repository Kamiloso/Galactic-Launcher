namespace GalacticLauncher.Core.DbModels;

// Represents the 'games_tags' junction table
public record GameTag
{
    public required long IdGame { get; init; }
    public required long IdTag { get; init; }
}
