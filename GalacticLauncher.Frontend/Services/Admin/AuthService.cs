using GalacticLauncher.Core.Dto;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Tools.Networking;
using System;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Services.Admin;

public interface IAuthService
{
    string Username { get; }
    string Token { get; }
    bool IsValidSession { get; }

    Task<bool> TryAuthenticateAsync(string username, string password);
    AdminBox<T> MakeAdminBox<T>(T body);
    TimeSpan TimeToExpiration();
}

public class AuthService(
    IErrorHandler errorHandler,
    IBackendTalker backendTalker) : IAuthService
{
    public string Username { get; private set; } = "";
    public string Token { get; private set; } = "";
    public bool IsValidSession => TimeToExpiration() > TimeSpan.Zero;

    public async Task<bool> TryAuthenticateAsync(string username, string password)
    {
        LoginRequest request = new()
        {
            Username = username,
            Password = password
        };

        try
        {
            LoginResult result = await backendTalker.GetAdminToken(request);

            if (!result.Authenticated)
                return false;

            Username = username;
            Token = result.Token;

            return true;
        }
        catch (ApiException ex)
        {
            errorHandler.HandleApiError(
                ex.StatusCode, showNoInternet: true);

            return false;
        }
    }

    public AdminBox<T> MakeAdminBox<T>(T body)
    {
        return new AdminBox<T>
        {
            Token = Token,
            Body = body
        };
    }

    public TimeSpan TimeToExpiration()
    {
        if (!Token.Contains('|'))
            return TimeSpan.Zero;

        if (!long.TryParse(Token.Split('|')[1], out long ticks))
            return TimeSpan.Zero;

        TimeSpan timeLeft = new DateTime(ticks) - DateTime.UtcNow;

        return timeLeft > TimeSpan.Zero
            ? timeLeft
            : TimeSpan.Zero;
    }
}
