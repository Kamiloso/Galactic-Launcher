namespace GalacticLauncher.Core.Models;

public record Game : GameRaw
{
    public required string? IconUrl { get; init; }
    public required string? TagIdList { get; init; } // "1,2,3,4" etc.

    public long[] ExtractTagIds()
    {
        if (TagIdList == null)
            return [];

        return [.. TagIdList.Split(',')
            .Where(arg => long.TryParse(arg, out _))
            .Select(long.Parse)];
    }

    protected static T InjectInternal<T>(T robustGame, Game game) where T : Game
    {
        return robustGame with
        {
            Id = game.Id,
            Name = game.Name,
            Author = game.Author,
            Description = game.Description,
            IconUrl = game.IconUrl,
            TagIdList = game.TagIdList,
        };
    }

    public static Game GetFallback(long id)
    {
        return new Game
        {
            Id = id,
            Name = "Unknown",
            Description = "",
            Author = "Unknown",
            IconUrl = null,
            TagIdList = null,
        };
    }
}
