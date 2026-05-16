using Dapper;
using GalacticLauncher.Core.DbModels;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface IImageRepository
{
    Task<IEnumerable<Image>> GetImagesByGameId(long gameId);
}

public class ImageRepository(MySqlConnection db) : IImageRepository
{
    public async Task<IEnumerable<Image>> GetImagesByGameId(long gameId)
    {
        return await db.QueryAsync<Image>("""
            SELECT * FROM images
                WHERE id_game = @p1
            """,
            new { p1 = gameId }
        );
    }
}
