namespace GalacticLauncher.Core.Models;

public record Tag
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}
