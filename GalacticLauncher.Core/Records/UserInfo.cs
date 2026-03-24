namespace GalacticLauncher.Core.Records;

internal record UserInfo(
    ulong Id, // PK
    string GoogleKey, // google auth identity
    string Email, // email
    string Name, // name
    string ProfileUrl // url to profile picture
    );
