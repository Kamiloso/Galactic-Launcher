namespace GalacticLauncher.Core.Models;

public record History
{
    public required long Id { get; init; }
    public required string Info { get; init; }
    public required DateTime Timestamp { get; init; }
    public required long? IdGame { get; init; }
}
