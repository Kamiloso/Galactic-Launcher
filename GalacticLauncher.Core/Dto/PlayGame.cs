namespace GalacticLauncher.Core.Dto;

public record PlayGame
{
    public required long GameId { get; init; }
    public required string GameName { get; init; }
    public required long VersionId { get; init; }
    public required string VersionName { get; init; }

    public PlayGame Sanitize()
    {
        const int MAX_LENGTH = 128;

        return this with
        {
            GameName = GameName.Length > MAX_LENGTH
                ? GameName[..MAX_LENGTH]
                : GameName,

            VersionName = VersionName.Length > MAX_LENGTH
                ? VersionName[..MAX_LENGTH]
                : VersionName,
        };
    }
}
