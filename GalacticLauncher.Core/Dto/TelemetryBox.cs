namespace GalacticLauncher.Core.Dto;

public record TelemetryBox
{
    public required Guid Guid { get; init; }
}

public record TelemetryBox<T> : TelemetryBox
{
    public required T Body { get; init; }
}
