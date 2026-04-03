using Dapper;
using GalacticLauncher.Core.DbRecords;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

// TODO: Divide it into separate repositories for each record type

public class RepoMock(
    MySqlConnection db
    ) : IGameRepository, IImageRepository, IVersionRepository, IExecRepository
{
    public async Task<IEnumerable<GameInfo>> GetAllGames()
    {
        return await db.QueryAsync<GameInfo>(
            "SELECT * FROM games"
            );
    }

    public async Task<IEnumerable<ImageInfo>> GetImagesByGameId(ulong gameId)
    {
        return await db.QueryAsync<ImageInfo>(
            "SELECT * FROM images WHERE IdGame = @p1",
            new { p1 = gameId }
            );
    }

    public async Task<IEnumerable<VersionInfo>> GetVersionsByGameId(ulong gameId)
    {
        return await db.QueryAsync<VersionInfo>(
            "SELECT * FROM versions WHERE IdGame = @p1",
            new { p1 = gameId }
            );
    }

    public async Task<VersionInfo?> GetPrimaryVersion(ulong gameId)
    {
        return await db.QueryFirstOrDefaultAsync<VersionInfo>(
            "SELECT * FROM versions WHERE IdGame = @p1 AND IsPrimary = TRUE",
            new { p1 = gameId }
            );
    }

    public async Task<IEnumerable<ExecInfo>> GetExecsByVersionId(ulong versionId)
    {
        return await db.QueryAsync<ExecInfo>(
            "SELECT * FROM execs WHERE IdVersion = @p1",
            new { p1 = versionId }
            );
    }
}
