using Dapper;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Infrastructure;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories.Readers;

public interface IVersionReader
{
    Task<VersionEntity?> GetVersionById(long id);
    Task<IEnumerable<VersionEntity>> GetVersionsByGameId(long idGame);
}

internal class VersionReader(DbSession session) : IVersionReader
{
    private readonly MySqlConnection _db = session.Connection;

    public async Task<VersionEntity?> GetVersionById(long id)
    {
        return await _db.QuerySingleOrDefaultAsync<VersionEntity>(
            "SELECT * FROM versions WHERE id = @p1",
            new { p1 = id },
            transaction: session.Transaction);
    }

    public async Task<IEnumerable<VersionEntity>> GetVersionsByGameId(long idGame)
    {
        return await _db.QueryAsync<VersionEntity>(
            "SELECT * FROM versions WHERE id_game = @p1",
            new { p1 = idGame },
            transaction: session.Transaction);
    }
}
