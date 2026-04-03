namespace GalacticLauncher.Core.DbRecords;

public record ImageInfo
{
    public required long Id { get; init; } // PK
    public required string Url { get; init; } // where to download from
    public required long IdGame { get; init; } // FK
}