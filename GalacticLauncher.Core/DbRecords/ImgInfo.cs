namespace GalacticLauncher.Core.DbRecords;

public record ImgInfo
{
    public required ulong Id { get; init; } // PK
    public required string Url { get; init; } // where to download from
    public required ulong IdGame { get; init; } // FK
}