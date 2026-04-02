namespace GalacticLauncher.Core.DbRecords;

public record GameTagInfo
{
    public required ulong IdGame { get; init; }
    public required ulong IdTag { get; init; }
}