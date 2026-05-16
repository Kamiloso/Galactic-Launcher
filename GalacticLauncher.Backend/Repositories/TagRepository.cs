using Dapper;
using GalacticLauncher.Core.DbModels;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetAllTags();
    Task<IEnumerable<Tag>> GetTagsByGameId(long gameId);
}

public class TagRepository(MySqlConnection db) : ITagRepository
{
    public async Task<IEnumerable<Tag>> GetAllTags()
    {
        return await db.QueryAsync<Tag>(
            "SELECT * FROM tags"
        );
    }
    public async Task<IEnumerable<Tag>> GetTagsByGameId(long gameId)
    {
        return await db.QueryAsync<Tag>("""
            SELECT * FROM tags
                JOIN games_tags ON tags.id = games_tags.id_tag
                WHERE games_tags.id_game = @p1
            """,
            new { p1 = gameId }
        );
    }
}
