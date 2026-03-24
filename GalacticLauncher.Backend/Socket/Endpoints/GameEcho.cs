using GalacticLauncher.Core;
using GalacticLauncher.Core.Records;
using Microsoft.Extensions.Logging;

namespace GalacticLauncher.Backend.Socket.Endpoints;

internal class GameEcho : IEndpoint
{
    public string Name => "game-echo";
    public string HttpMethod => "POST";
    public string? RateLimitingPolicy => Def.LOW_COST;
    public string Summary => "Sends back a given game.";
    public Delegate HandleRequest => (
        GameInfo gameInfo
        // you can put here any object - this is veery flexible
        // works for: logger, objects deserializable from json and even request parameters
        ) =>
    {
        return gameInfo;
    };
}