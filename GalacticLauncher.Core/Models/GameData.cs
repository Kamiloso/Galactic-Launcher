namespace GalacticLauncher.Core.Models;

public record GameData : Game
{
    public required IEnumerable<Version> Versions { get; init; }
    public required IEnumerable<Image> Images { get; init; }

    public GameData Inject(Game game)
    {
        return InjectInternal(this, game);
    }
}
