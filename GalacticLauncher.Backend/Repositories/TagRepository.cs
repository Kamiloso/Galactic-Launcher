using Dapper;
using GalacticLauncher.Core.DbRecords;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public class TagRepository(MySqlConnection db) : ITagRepository
{
    public async Task<IEnumerable<TagInfo>> GetAllTags()
    {
        return await db.QueryAsync<TagInfo>(
            "SELECT * FROM games"
        );
    }
    public async Task<IEnumerable<TagInfo>> GetTagsByGameId(long gameId)
    {
        return await db.QueryAsync<TagInfo>(
            @"SELECT t.* FROM tags t INNER JOIN games_tags gt ON t.id = gt.id_tag WHERE gt.id_game = @p1",
            new { p1 = gameId }
        );
    }
}
