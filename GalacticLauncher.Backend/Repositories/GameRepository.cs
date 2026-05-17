using Dapper;
using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Backend.Models;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface IGameRepository
{
    Task<GameWithIconEntity?> GetGameById(long id);
    Task<IEnumerable<GameWithIconEntity>> GetAllGames();
    Task<IEnumerable<GameWithIconEntity>> GetAllGamesWithTagContraints(IEnumerable<long> tagIds);
}

internal class GameRepository(DbSession session) : IGameRepository
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

    public async Task<IEnumerable<GameWithIconEntity>> GetAllGamesWithTagContraints(IEnumerable<long> tagIds)
    {
        if (!tagIds.Any())
            return await GetAllGames();

        return await _db.QueryAsync<GameWithIconEntity>($"""
            WITH temp AS (
                SELECT games.* FROM games
                    JOIN games_tags ON games.id = games_tags.id_game
                    WHERE games_tags.id_tag IN @p1
                    GROUP BY games.id
                    HAVING COUNT(*) = @p2
            )
            {ICON_SEARCH("temp")}
            """,
            new { p1 = tagIds, p2 = tagIds.Count() },
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
