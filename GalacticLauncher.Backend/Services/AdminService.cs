using GalacticLauncher.Core.Dto;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace GalacticLauncher.Backend.Services;

public interface IAdminService
{
    LoginResult AuthenticateAdmin(LoginRequest loginRequest);
    bool TryValidateSession(string token, out string username);
}

internal class AdminService(AppConfig config) : IAdminService
{
    private readonly TimeSpan GRACE_PERIOD = TimeSpan.FromSeconds(config.Admin.GracePeriodSeconds);

    private readonly ConcurrentDictionary<string, AdminSession> _sessions = new();

    private record AdminSession(
        string Username, DateTime Expiration
    );

    public LoginResult AuthenticateAdmin(LoginRequest loginRequest)
    {
        string username = loginRequest.Username;
        string password = loginRequest.Password;

        if (string.IsNullOrEmpty(username)) return LoginResult.Failed();
        if (string.IsNullOrEmpty(password)) return LoginResult.Failed();

        List<LoginRequest> allowedRequests = [.. config.Admin.Logins
            .Select(rc => new LoginRequest
            {
                Username = rc.Username,
                Password = rc.Password
            })];

        if (allowedRequests.Any(req => req == loginRequest))
        {
            string token = CreateSession(username);
            return LoginResult.Success(token);
        }

        return LoginResult.Failed();
    }

    private string CreateSession(string username)
    {
        CleanupExpiredSessions();

        AdminSession session = new(
            Username: username,
            Expiration: DateTime.UtcNow.AddSeconds(
                config.Admin.AdminSessionSeconds)
            );

        byte[] entropy = RandomNumberGenerator.GetBytes(32);

        string randomPart = Convert.ToBase64String(entropy);
        string expirationPart = session.Expiration.Ticks.ToString();

        string token = $"{randomPart}|{expirationPart}";

        _sessions.TryAdd(token, session);

        return token;
    }

    public bool TryValidateSession(string token, out string username)
    {
        if (_sessions.TryGetValue(token, out var session))
        {
            if (session.Expiration > DateTime.UtcNow + GRACE_PERIOD)
            {
                username = session.Username;
                return true;
            }
        }

        username = null!;
        return false;
    }

    private void CleanupExpiredSessions()
    {
        DateTime now = DateTime.UtcNow;

        foreach (var kvp in _sessions)
        {
            if (kvp.Value.Expiration <= now)
            {
                _sessions.TryRemove(kvp.Key, out _);
            }
        }
    }
}
