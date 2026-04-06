using Dapper;
using GalacticLauncher.Core.DbRecords;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;
public class ImageRepository(MySqlConnection db) : IImageRepository
{
    public async Task<IEnumerable<ImageInfo>> GetImagesByGameId(long gameId)
    {
        return await db.QueryAsync<ImageInfo>(
            "SELECT * FROM images WHERE id_game = @p1",
            new { p1 = gameId }
        );
    }
}
