using Dapper;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Infrastructure;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface ITagRepository
{
    Task<TagEntity?> GetTagById(int id);
    Task<IEnumerable<TagEntity>> GetAllTags();
    Task<IEnumerable<TagEntity>> GetTagsByGameId(long idGame);
    Task<long> CreateTag(TagEntity tag);
    Task DeleteTagById(long idTag);
}

internal class TagRepository(DbSession session) : ITagRepository
{
    private readonly MySqlConnection _db = session.Connection;

    public async Task<TagEntity?> GetTagById(int id)
    {
        return await _db.QuerySingleOrDefaultAsync<TagEntity>(
            "SELECT * FROM tags WHERE id = @p1",
            new { p1 = id },
            transaction: session.Transaction);
    }

    public async Task<IEnumerable<TagEntity>> GetAllTags()
    {
        return await _db.QueryAsync<TagEntity>(
            "SELECT * FROM tags",
            transaction: session.Transaction);
    }

    public async Task<IEnumerable<TagEntity>> GetTagsByGameId(long idGame)
    {
        return await _db.QueryAsync<TagEntity>("""
            SELECT * FROM tags
                JOIN games_tags ON tags.id = games_tags.id_tag
                WHERE games_tags.id_game = @p1
            """,
            new { p1 = idGame },
            transaction: session.Transaction);
    }

    public async Task<long> CreateTag(TagEntity tag)
    {
        return await _db.QueryFirstAsync<long>("""
            INSERT INTO tags
                (name, description) VALUES
                (@Name, @Description);
            SELECT LAST_INSERT_ID();
            """,
            tag,
            transaction: session.Transaction);
    }

    public async Task DeleteTagById(long idTag)
    {
        await _db.ExecuteAsync(
            "DELETE FROM tags WHERE id = @p1",
            new { p1 = idTag },
            transaction: session.Transaction);
    }
}
