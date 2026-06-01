namespace GalacticLauncher.Core.Dto;

public record Telemetry
{
    public required Guid Guid { get; init; }
}

public record Telemetry<T> : Telemetry
{
    public required T Body { get; init; }
}
