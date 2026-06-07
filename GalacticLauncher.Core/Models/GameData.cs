namespace GalacticLauncher.Core.Models;

public record GameData : Game
{
    public required IEnumerable<Version> Versions { get; init; }
    public required IEnumerable<Image> Images { get; init; }

    public GameData Inject(Game game)
    {
        return InjectInternal(this, game);
    }

    public GameData RemoveIncompatiblePlatforms()
    {
        return this with
        {
            Versions = [.. Versions
                .Where(ver => ver.Platform == Utils.CurrentPlatform)]
        };
    }
}
