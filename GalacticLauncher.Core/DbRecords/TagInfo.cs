namespace GalacticLauncher.Core.DbRecords;

public record TagInfo
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}