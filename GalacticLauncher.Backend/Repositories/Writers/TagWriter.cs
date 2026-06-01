using Dapper;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Infrastructure;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories.Writers;

public interface ITagWriter
{
    Task<long> CreateTag(TagEntity tag);
    Task DeleteTagById(long idTag);
}

internal class TagWriter(DbSession session) : ITagWriter
{
    private readonly MySqlConnection _db = session.Connection;

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
