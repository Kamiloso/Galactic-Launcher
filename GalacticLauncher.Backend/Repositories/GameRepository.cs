using Dapper;
using GalacticLauncher.Core.DbModels;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface IGameRepository
{
    Task<IEnumerable<long>> GetAllGameIds();
    Task<Game> GetGameById(long id);
}

public class GameRepository(MySqlConnection db) : IGameRepository
{
    public async Task<IEnumerable<long>> GetAllGameIds()
    {
        return await db.QueryAsync<long>(
            "SELECT id FROM games"
            );
    }

    public async Task<Game> GetGameById(long id)
    {
        return await db.QuerySingleAsync<Game>("""
            SELECT * FROM games
                WHERE id = @id
            """,
            new { id }
            );
    }
}
