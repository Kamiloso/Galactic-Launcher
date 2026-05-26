using GalacticLauncher.Core.Models;
using System.Collections.Immutable;

namespace GalacticLauncher.Frontend.Domain.Models;

internal record CacheStorage
{
    public required ImmutableArray<Game> GameCache { get; init; }
    public required ImmutableArray<GameData> GameDataCache { get; init; }

}
