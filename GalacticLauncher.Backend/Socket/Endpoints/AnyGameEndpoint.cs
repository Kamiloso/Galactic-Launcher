using GalacticLauncher.Core;
using GalacticLauncher.Core.Enums;
using GalacticLauncher.Core.Records;

namespace GalacticLauncher.Backend.Socket.Endpoints;

internal class AnyGameEndpoint : IEndpoint
{
    public string Name => "anygame";
    public string HttpMethod => "GET";
    public string? RateLimitingPolicy => Def.LOW_COST;
    public string Summary => "Returns random game information.";

    public Delegate HandleRequest => (
        ILogger<AnyGameEndpoint> logger
        ) =>
    {
        Random rnd = new();
        int r = rnd.Next(0, 5);

        return new GameInfo(
            r switch
            {
                0 => "Space Eternity 3",
                1 => "Brick Breaker",
                2 => "Maze Game",
                3 => "Rollball",
                4 => "Larnix",
                _ => string.Empty,
            },
            "A random game.",
            GameTag.Action | GameTag.Adventure,
            [
                new VersionInfo("1.0", "Official release", DateTime.Now, VersionType.Release, [], [])
            ],
            [
                "random-url"
            ]
            );
    };
}