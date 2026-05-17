namespace GalacticLauncher.Core.Dto;

public record LoginResult
{
    public required bool Authenticated { get; init; }
    public required string Token { get; init; }

    public static LoginResult Success(string token) => new()
    {
        Authenticated = true,
        Token = token
    };

    public static LoginResult Failed() => new()
    {
        Authenticated = false,
        Token = ""
    };
}
