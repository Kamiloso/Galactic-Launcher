using Dapper;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Infrastructure;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories.Readers;

public interface ITagReader
{
    Task<TagEntity?> GetTagById(int id);
    Task<IEnumerable<TagEntity>> GetAllTags();
    Task<IEnumerable<TagEntity>> GetTagsByGameId(long idGame);
}

internal class TagReader(DbSession session) : ITagReader
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
}
