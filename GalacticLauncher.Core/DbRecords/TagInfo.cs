namespace GalacticLauncher.Core.DbRecords;

public record TagInfo
{
    public required ulong Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public ICollection<GameInfo> Games { get; init; } = [];
}