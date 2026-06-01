namespace GalacticLauncher.Backend.Domain.Models;

// Represents the 'history' table
public record HistoryEntity
{
    public required long Id { get; init; }
    public required string Info { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.Now;
    public long? IdGame { get; init; } = null; 
}
