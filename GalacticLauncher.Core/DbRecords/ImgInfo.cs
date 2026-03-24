namespace GalacticLauncher.Core.DbRecords;

internal record ImgInfo(
    ulong Id, // PK
    string Url, // where to download from
    ulong IdGame // FK
    );
