using Dapper;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Infrastructure;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface IGameRepository
{
    Task<GamePlusEntity?> GetGameById(long id);
    Task<IEnumerable<GamePlusEntity>> GetAllGames();
    Task<long> CreateGame(GameEntity game);
    Task DeleteGameById(long idGame);
}

internal class GameRepository(DbSession session) : IGameRepository
{
    private readonly MySqlConnection _db = session.Connection;

    public async Task<GamePlusEntity?> GetGameById(long id)
    {
        return await _db.QuerySingleOrDefaultAsync<GamePlusEntity>($"""
            WITH temp AS (
                SELECT * FROM games
                    WHERE id = @p1
            )
            {SEARCH_FOR_MORE("temp")}
            """,
            new { p1 = id },
            transaction: session.Transaction);
    }

    public async Task<IEnumerable<GamePlusEntity>> GetAllGames()
    {
        return await _db.QueryAsync<GamePlusEntity>(
            $"{SEARCH_FOR_MORE("games")}",
            transaction: session.Transaction);
    }

    public async Task<long> CreateGame(GameEntity game)
    {
        return await _db.QueryFirstAsync<long>("""
            INSERT INTO games
                (name, author, description) VALUES
                (@Name, @Author, @Description);
            SELECT LAST_INSERT_ID();
            """,
            game,
            transaction: session.Transaction);
    }

    public async Task DeleteGameById(long idGame)
    {
        await _db.ExecuteAsync(
            "DELETE FROM games WHERE id = @p1",
            new { p1 = idGame },
            transaction: session.Transaction);
    }

    private static string SEARCH_FOR_MORE(string table) => $"""
        SELECT
            tmp.*,
            tags_agg.tag_id_list
        FROM (
            SELECT
                {table}.*,
                images.download_url AS icon_url
            FROM {table}
            LEFT JOIN images ON
                {table}.id = images.id_game
                AND images.type = 'icon'
                AND images.id = (
                    SELECT MAX(id)
                    FROM images i2
                    WHERE i2.id_game = {table}.id
                      AND i2.type = 'icon'
                )
        ) AS tmp
        LEFT JOIN (
            SELECT
                id_game,
                GROUP_CONCAT(id_tag) AS tag_id_list
            FROM games_tags
            GROUP BY id_game
        ) AS tags_agg ON tmp.id = tags_agg.id_game;
        """;
}
