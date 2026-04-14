using Dapper;
using GalacticLauncher.Core.DbRecords;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface IVersionRepository
{
    Task<IEnumerable<VersionInfo>> GetVersionsByGameId(long gameId);
    Task<VersionInfo?> GetPrimaryVersion(long gameId);
}

public class VersionRepository(MySqlConnection db) : IVersionRepository
{
    public async Task<IEnumerable<VersionInfo>> GetVersionsByGameId(long gameId)
    {
        return await db.QueryAsync<VersionInfo>(
            "SELECT * FROM versions WHERE id_game = @p1",
            new { p1 = gameId }
        );
    }

    public async Task<VersionInfo?> GetPrimaryVersion(long gameId)
    {
        return await db.QueryFirstOrDefaultAsync<VersionInfo>(
            "SELECT * FROM versions WHERE id_game = @p1 AND is_primary = TRUE",
            new { p1 = gameId }
        );
    }
}
