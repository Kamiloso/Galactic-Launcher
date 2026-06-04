namespace GalacticLauncher.Core.Models;

public record GameTree : GameRaw
{
    public required IEnumerable<Version> Versions { get; init; }
    public required IEnumerable<Image> Images { get; init; }
    public required IEnumerable<long> TagIds { get; init; }
}
