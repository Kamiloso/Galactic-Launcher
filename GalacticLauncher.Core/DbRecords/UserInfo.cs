namespace GalacticLauncher.Core.DbRecords;

public record UserInfo
{
    public ulong Id { get; init; } // PK
    public string GoogleKey { get; init; } = string.Empty; // google auth identity
    public string Email { get; init; } = string.Empty; // email
    public string Name { get; init; } = string.Empty; // full name
    public string ProfileUrl { get; init; } = string.Empty; // url to profile picture
    // TODO: Add the tags logic
}