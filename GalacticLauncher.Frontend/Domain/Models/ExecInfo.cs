namespace GalacticLauncher.Frontend.Domain.Models;

/// <summary>
/// Represents game data that is necessary while working with game executables.
/// </summary>
public record GameInfo
{
    public required string GameUnique { get; init; }
}
