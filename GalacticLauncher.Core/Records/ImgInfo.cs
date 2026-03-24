namespace GalacticLauncher.Core.Records;

internal record ImgInfo(
    ulong Id, // PK
    string Url,
    ulong IdGame // FK
    );
