namespace GalacticLauncher.Frontend.Domain.Models;

public record GameDisplay
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string? IconUrl { get; init; }

    // TODO: Add more fields that may be useful for display,
    // including author, version, release date, etc.
}
