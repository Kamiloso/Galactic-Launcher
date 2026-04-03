namespace GalacticLauncher.Backend;

public record AppConfig
{
    public required ListenerSection Listener { get; init; }
    public required DatabaseSection Database { get; init; }
    public required RateLimitingSection Limiter { get; init; }

    public record ListenerSection
    {
        public required int PrefixIPv4 { get; init; }
        public required int PrefixIPv6 { get; init; }
        public required bool UseForwardedFor { get; init; }
    }

    public record DatabaseSection
    {
        public required string Address { get; init; }
        public required ushort Port { get; init; }
        public required string Database { get; init; }
        public required string User { get; init; }
        public required string Password { get; init; }
    }

    public record RateLimitingSection
    {
        public required RateLimitRule LowCost { get; init; }
        public required RateLimitRule MediumCost { get; init; }
        public required RateLimitRule HighCost { get; init; }

        public record RateLimitRule
        {
            public required int Seconds { get; init; }
            public required int Limit { get; init; }
        }
    }
}