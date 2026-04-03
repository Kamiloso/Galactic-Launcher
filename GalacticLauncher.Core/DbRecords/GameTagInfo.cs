namespace GalacticLauncher.Core.DbRecords;

public record GameTagInfo
{
    public required long IdGame { get; init; }
    public required long IdTag { get; init; }
}