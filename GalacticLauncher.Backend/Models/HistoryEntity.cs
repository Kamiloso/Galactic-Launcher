namespace GalacticLauncher.Backend.Models;

// Represents the 'history' table
public record HistoryEntity
{
    public long Id { get; init; } = 0; // auto_increment
    public required string Info { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.Now;
    public long? IdGame { get; init; } = null; 
}
