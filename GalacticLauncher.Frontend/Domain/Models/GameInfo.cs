namespace GalacticLauncher.Frontend.Domain.Models;

/// <summary>
/// Represents game data that is necessary while working with game executables.
/// </summary>
public record GameInfo
{
    public required long GameId { get; init; }
    public required string GameName { get; init; }
    public required string GameUnique { get; init; }
}
