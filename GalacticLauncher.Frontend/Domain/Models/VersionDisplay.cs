using System;

namespace GalacticLauncher.Frontend.Domain.Models;

public record VersionDisplay
{
    public required long Id { get; init; }
    public required string Caption { get; init; }
    public required string Description { get; init; }
    public required bool IsPrimary { get; init; }
    public required DateOnly ReleaseDate { get; init; }

    // TODO: Add more fields that may be useful for display,
    // including author, version, release date, etc.
}
