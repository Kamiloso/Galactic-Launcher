using Dapper;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Infrastructure;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories.Writers;

public interface IGameWriter
{
    Task<long> CreateGame(GameEntity game);
    Task DeleteGameById(long idGame);
}

internal class GameWriter(DbSession session) : IGameWriter
{
    private readonly MySqlConnection _db = session.Connection;

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
}
