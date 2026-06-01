using Dapper;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Infrastructure;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories.Readers;

public interface IGameReader
{
    Task<GameWithIconEntity?> GetGameById(long id);
    Task<IEnumerable<GameWithIconEntity>> GetAllGames();
}

internal class GameReader(DbSession session) : IGameReader
{
    private readonly MySqlConnection _db = session.Connection;

    public async Task<GameWithIconEntity?> GetGameById(long id)
    {
        return await _db.QuerySingleOrDefaultAsync<GameWithIconEntity>($"""
            WITH temp AS (
                SELECT * FROM games
                    WHERE id = @p1
            )
            {ICON_SEARCH("temp")}
            """,
            new { p1 = id },
            transaction: session.Transaction);
    }

    public async Task<IEnumerable<GameWithIconEntity>> GetAllGames()
    {
        return await _db.QueryAsync<GameWithIconEntity>(
            $"{ICON_SEARCH("games")}",
            transaction: session.Transaction);
    }

    private static string ICON_SEARCH(string table) => $"""
        SELECT {table}.*, images.download_url AS icon_url
            FROM {table}
            LEFT JOIN images ON
                {table}.id = images.id_game AND
                images.type = 'icon' AND
                images.id = (
                    SELECT MAX(id)
                    FROM images i2
                    WHERE
                        i2.id_game = {table}.id AND
                        i2.type = 'icon'
                )
        """;
}
