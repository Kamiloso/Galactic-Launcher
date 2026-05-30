namespace GalacticLauncher.Core.Models;

public record Game
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Author { get; init; }
    public required string Description { get; init; }
    public required string? IconUrl { get; init; }

    public static bool FlatEquals(Game? game1, Game? game2)
    {
        int nullCount =
            (game1 is null ? 1 : 0) +
            (game2 is null ? 1 : 0);

        if (nullCount == 2) return true;
        if (nullCount == 1) return false;

        return game1!.Id == game2!.Id
            && game1.Name == game2.Name
            && game1.Author == game2.Author
            && game1.Description == game2.Description
            && game1.IconUrl == game2.IconUrl;
    }
}
