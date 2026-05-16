using Dapper;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface IVersionRepository
{
    Task<IEnumerable<Version>> GetVersionsByGameId(long gameId);
}

public class VersionRepository(MySqlConnection db) : IVersionRepository
{
    public async Task<IEnumerable<Version>> GetVersionsByGameId(long gameId)
    {
        return await db.QueryAsync<Version>("""
            SELECT * FROM versions
                WHERE id_game = @p1
            """,
            new { p1 = gameId }
        );
    }
}
