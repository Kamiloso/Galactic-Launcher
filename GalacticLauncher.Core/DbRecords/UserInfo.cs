namespace GalacticLauncher.Core.DbRecords;

internal record UserInfo(
    ulong Id, // PK
    string GoogleKey, // google auth identity
    string Email, // email
    string Name, // full name
    string ProfileUrl // url to profile picture
    );
