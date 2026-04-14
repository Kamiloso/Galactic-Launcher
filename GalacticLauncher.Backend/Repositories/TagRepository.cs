using Dapper;
using GalacticLauncher.Core.DbRecords;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface ITagRepository
{
    Task<IEnumerable<TagInfo>> GetAllTags();
    Task<IEnumerable<TagInfo>> GetTagsByGameId(long gameId);
}

public class TagRepository(MySqlConnection db) : ITagRepository
{
    public async Task<IEnumerable<TagInfo>> GetAllTags()
    {
        return await db.QueryAsync<TagInfo>(
            "SELECT * FROM tags"
        );
    }
    public async Task<IEnumerable<TagInfo>> GetTagsByGameId(long gameId)
    {
        return await db.QueryAsync<TagInfo>("""
            SELECT * FROM tags
                INNER JOIN games_tags ON tags.id = games_tags.id_tag
                WHERE games_tags.id_game = @p1
            """,
            new { p1 = gameId }
        );
    }
}
