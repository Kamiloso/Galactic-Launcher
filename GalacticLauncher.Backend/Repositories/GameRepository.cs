using Dapper;
using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Backend.Models;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface IGameRepository
{
    Task<GameEntity> GetGameById(long id);
    Task<IEnumerable<GameEntity>> GetAllGames();
    Task<IEnumerable<GameWithIconEntity>> GetAllGamesWithIcons();
    Task<IEnumerable<GameEntity>> GetGamesByTagIds(IEnumerable<long> tagIds);
}

internal class GameRepository(DbSession session) : IGameRepository
{
    private readonly MySqlConnection _db = session.Connection;

    public async Task<GameEntity> GetGameById(long id)
    {
        return await _db.QuerySingleAsync<GameEntity>(
            "SELECT * FROM games WHERE id = @p1",
            new { p1 = id },
            transaction: session.Transaction
            );
    }

    public async Task<IEnumerable<GameEntity>> GetAllGames()
    {
        return await _db.QueryAsync<GameEntity>(
            "SELECT * FROM games",
            transaction: session.Transaction
            );
    }

    public async Task<IEnumerable<GameWithIconEntity>> GetAllGamesWithIcons()
    {
        return await _db.QueryAsync<GameWithIconEntity>("""
            SELECT games.*, images.download_url AS icon_url
                FROM games
                LEFT JOIN images ON
                    games.id = images.id_game AND
                    images.type = 'icon' AND
                    images.id = (
                        SELECT MAX(id)
                        FROM images i2
                        WHERE
                            i2.id_game = games.id AND
                            i2.type = 'icon'
                    )
            """,
            transaction: session.Transaction
            );
    }

    public async Task<IEnumerable<GameEntity>> GetGamesByTagIds(IEnumerable<long> tagIds)
    {
        return await _db.QueryAsync<GameEntity>(
            """
            SELECT * FROM games
                JOIN games_tags ON games.id = games_tags.id_game
                WHERE games_tags.id_tag IN @p1
            """,
            new { p1 = tagIds.ToArray() },
            transaction: session.Transaction
            );
    }
}
