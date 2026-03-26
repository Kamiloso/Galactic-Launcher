namespace GalacticLauncher.Backend;

public class AppConfig
{
    public required ListenerSection ListenerConfig { get; init; }
    public required DatabaseSection DatabaseConfig { get; init; }
    public required RateLimitingSection LimiterPolicyConfig { get; init; }

    public class ListenerSection
    {
        public required int PrefixIPv4 { get; init; }
        public required int PrefixIPv6 { get; init; }
        public required bool UseForwardedFor { get; init; }
    }

    public class DatabaseSection
    {
        public required string Address { get; init; }
        public required int Port { get; init; }
        public required string Database { get; init; }
        public required string User { get; init; }
        public required string Password { get; init; }
    }

    public class RateLimitingSection
    {
        public required RateLimitRule LowCost { get; init; }
        public required RateLimitRule MediumCost { get; init; }
        public required RateLimitRule HighCost { get; init; }

        public class RateLimitRule
        {
            public required int Seconds { get; init; }
            public required int Limit { get; init; }
        }
    }

    public static AppConfig? ObtainFrom(IConfiguration configuration)
    {
        try
        {
            return configuration.Get<AppConfig>();
        }
        catch
        {
            return null;
        }
    }
}