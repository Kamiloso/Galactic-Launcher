using Dapper;
using GalacticLauncher.Backend.Infrastructure;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface IDbWriteLocker
{
    Task LockDatabase();
}

internal class DbWriteLocker(DbSession session) : IDbWriteLocker
{
    private readonly MySqlConnection _db = session.Connection;

    public async Task LockDatabase()
    {
        // TODO: Rethink it massively

        await _db.QueryAsync("""
            autocommit = 0;
            LOCK TABLES
                games WRITE,
                versions WRITE,
                images WRITE,
                tags WRITE,
                games_tags WRITE,
                history WRITE
            """,
            transaction: session.Transaction);
    }
}
