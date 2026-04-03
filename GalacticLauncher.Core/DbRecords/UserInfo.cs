namespace GalacticLauncher.Core.DbRecords;

public record UserInfo
{
    public required long Id { get; init; } // PK
    public required string GoogleKey { get; init; } // google auth identity
    public required string Email { get; init; } // email
    public required string Name { get; init; } // full name
    public required string ProfileUrl { get; init; } // url to profile picture
}