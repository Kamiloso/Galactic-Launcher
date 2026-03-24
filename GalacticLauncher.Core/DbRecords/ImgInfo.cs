namespace GalacticLauncher.Core.DbRecords;

public record ImgInfo
{
    public ulong Id { get; init; } // PK
    public string Url { get; init; } = string.Empty; // where to download from
    public ulong IdGame { get; init; } // FK
}