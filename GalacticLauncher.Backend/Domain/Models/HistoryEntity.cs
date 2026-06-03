namespace GalacticLauncher.Backend.Domain.Models;

// Represents the 'history' table
public record HistoryEntity
{
    public required long Id { get; init; }
    public required string Info { get; init; }
    public required DateTime Timestamp { get; init; }
    public required long? IdGame { get; init; }
}
