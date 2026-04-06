using Dapper;
using GalacticLauncher.Core.DbRecords;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

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
        return await db.QueryAsync<GameInfo>(
            @"SELECT g.* FROM games g INNER JOIN games_tags gt ON g.id = gt.id_game WHERE gt.id_tag = @p1",
            new { p1 = tagId }
        );
    }
}
