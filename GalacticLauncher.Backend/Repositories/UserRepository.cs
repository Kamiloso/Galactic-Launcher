using Dapper;
using GalacticLauncher.Core.DbRecords;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface IUserRepository
{
    Task<UserInfo?> GetUserByEmail(string email);
    Task<UserInfo?> GetUserByGoogleKey(string googleKey);
}

public class UserRepository(MySqlConnection db) : IUserRepository
{
    public async Task<UserInfo?> GetUserByEmail(string email)
    {
        return await db.QueryFirstOrDefaultAsync<UserInfo>(
            "SELECT * FROM users WHERE email = @p1",
            new { p1 = email }
        );
    }

    public async Task<UserInfo?> GetUserByGoogleKey(string googleKey)
    {
        return await db.QueryFirstOrDefaultAsync<UserInfo>(
            "SELECT * FROM users WHERE google_key = @p1",
            new { p1 = googleKey }
        );
    }
}
