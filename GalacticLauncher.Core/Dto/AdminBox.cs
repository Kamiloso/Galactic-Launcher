namespace GalacticLauncher.Core.Dto;

public record AdminBox
{
    public required string Token { get; init; }
}

public record AdminBox<T> : AdminBox
{
    public required T Body { get; init; }
}
