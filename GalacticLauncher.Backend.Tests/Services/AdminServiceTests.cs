using GalacticLauncher.Backend.Services;
using GalacticLauncher.Core.Dto;
using GalacticLauncher.Backend.Tests.Helpers;

namespace GalacticLauncher.Backend.Tests.Services;

public class AdminServiceTests
{
    private const string DefaultUsername = "test_admin";
    private const string DefaultPassword = "secure_password";

    private AdminService CreateSut(
        string adminUser = DefaultUsername, 
        string adminPass = DefaultPassword, 
        int sessionSeconds = 3600, 
        int gracePeriodSeconds = 0)
    {
        var config = TestDataHelper.CreateDummyConfig(adminUser, adminPass, sessionSeconds, gracePeriodSeconds);
        return new AdminService(config);
    }

    [Fact]
    public void AuthenticateAdmin_ValidCredentials_ReturnsSuccessWithToken()
    {
        var sut = CreateSut();
        var request = new LoginRequest { Username = DefaultUsername, Password = DefaultPassword };
        
        LoginResult result = sut.AuthenticateAdmin(request);

        Assert.True(result.Authenticated);
        Assert.Contains("|", result.Token);
    }

    [Fact]
    public void AuthenticateAdmin_InvalidCredentials_ReturnsFailed()
    {
        var sut = CreateSut();
        var request = new LoginRequest { Username = "hacker", Password = "wrong_password" };
        
        LoginResult result = sut.AuthenticateAdmin(request);

        Assert.False(result.Authenticated);
        Assert.Empty(result.Token);
    }

    [Theory]
    [InlineData("", "password")]
    [InlineData("admin", "")]
    [InlineData("", "")]
    public void AuthenticateAdmin_EmptyOrNullCredentials_ReturnsFailed(string testUsername, string testPassword)
    {
        var sut = CreateSut();
        var request = new LoginRequest { Username = testUsername, Password = testPassword };
        
        LoginResult result = sut.AuthenticateAdmin(request);

        Assert.False(result.Authenticated);
        Assert.Empty(result.Token);
    }

    [Fact]
    public void TryValidateSession_ValidToken_ReturnsTrueAndUsername()
    {
        var sut = CreateSut();
        var request = new LoginRequest { Username = DefaultUsername, Password = DefaultPassword };
        var loginResult = sut.AuthenticateAdmin(request);

        bool isValid = sut.TryValidateSession(loginResult.Token, out string username);

        Assert.True(isValid);
        Assert.Equal(DefaultUsername, username);
    }

    [Fact]
    public void AuthenticateAdmin_ValidCredentials_ReturnsSuccessWithProperlyFormattedToken()
    {
        var sut = CreateSut();
        var request = new LoginRequest { Username = "test_admin", Password = "secure_password" };
        
        LoginResult result = sut.AuthenticateAdmin(request);

        Assert.True(result.Authenticated);
        
        var parts = result.Token.Split('|');
        Assert.Equal(2, parts.Length);
        
        Span<byte> buffer = new Span<byte>(new byte[parts[0].Length]);
        Assert.True(Convert.TryFromBase64String(parts[0], buffer, out _), "Token prefix must be valid Base64");
        
        Assert.True(long.TryParse(parts[1], out long ticks), "Token suffix must be valid long ticks");
        Assert.True(ticks > 0);
    }

    [Fact]
    public void TryValidateSession_ExactBoundaryGracePeriod_ReturnsFalse()
    {
        var sut = CreateSut(sessionSeconds: -3, gracePeriodSeconds: 3);
        var request = new LoginRequest { Username = "test_admin", Password = "secure_password" };
        var loginResult = sut.AuthenticateAdmin(request);

        bool isValid = sut.TryValidateSession(loginResult.Token, out string username);

        Assert.False(isValid, "Session exactly on or past the boundary should be invalid.");
        Assert.Null(username);
    }
}