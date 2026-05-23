using System.Collections.Immutable;

namespace GalacticLauncher.Frontend.Domain.Models;

internal record DataStorage
{
    public required ImmutableArray<string> Keys;
    public required ImmutableArray<ImmutableArray<long>> Values;
}
