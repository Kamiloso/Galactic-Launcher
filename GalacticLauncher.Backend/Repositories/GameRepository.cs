using Dapper;
using GalacticLauncher.Core.DbRecords;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface IGameRepository
{
    Task<IEnumerable<GameInfo>> GetAllGames();
    Task<IEnumerable<GameInfo>> GetGamesByTagId(long tagId);
}

public class GameRepository(MySqlConnection db) : IGameRepository
{
    public async Task<IEnumerable<GameInfo>> GetAllGames()
    {
        return await db.QueryAsync<GameInfo>(
            "SELECT * FROM games"
            );
    }
    
    public async Task<IEnumerable<GameInfo>> GetGamesByTagId(long tagId)
    {
        return await db.QueryAsync<GameInfo>("""
            SELECT * FROM games
                INNER JOIN games_tags ON games.id = games_tags.id_game
                WHERE games_tags.id_tag = @p1
            """,
            new { p1 = tagId }
        );
    }
}
