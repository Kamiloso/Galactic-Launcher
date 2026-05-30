namespace GalacticLauncher.Core.Models;

public record Game
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Author { get; init; }
    public required string Description { get; init; }
    public required string? IconUrl { get; init; }

    protected static T InjectInternal<T>(T robustGame, Game game) where T : Game
    {
        return robustGame with
        {
            Id = game.Id,
            Name = game.Name,
            Author = game.Author,
            Description = game.Description,
            IconUrl = game.IconUrl
        };
    }
}
