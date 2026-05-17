using Dapper;
using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Backend.Models;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface IImageRepository
{
    Task<ImageEntity?> GetImageById(long id);
    Task<IEnumerable<ImageEntity>> GetImagesByGameId(long idGame);
}

internal class ImageRepository(DbSession session) : IImageRepository
{
    private readonly MySqlConnection _db = session.Connection;

    public async Task<ImageEntity?> GetImageById(long id)
    {
        return await _db.QuerySingleOrDefaultAsync<ImageEntity>(
            "SELECT * FROM images WHERE id = @p1",
            new { p1 = id },
            transaction: session.Transaction);
    }

    public async Task<IEnumerable<ImageEntity>> GetImagesByGameId(long idGame)
    {
        return await _db.QueryAsync<ImageEntity>(
            "SELECT * FROM images WHERE id_game = @p1",
            new { p1 = idGame },
            transaction: session.Transaction);
    }
}
