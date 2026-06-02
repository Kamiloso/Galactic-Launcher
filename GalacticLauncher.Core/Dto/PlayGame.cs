using GalacticLauncher.Core.Models;
using Version = GalacticLauncher.Core.Models.Version;

namespace GalacticLauncher.Core.Dto;

public record PlayGame
{
    public required Game Game { get; init; }
    public required Version Version { get; init; }
}
