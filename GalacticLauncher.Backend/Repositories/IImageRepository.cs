using GalacticLauncher.Core.DbRecords;

namespace GalacticLauncher.Backend.Repositories;

public interface IImageRepository
{
    Task<IEnumerable<ImageInfo>> GetImagesByGameId(ulong gameId);
}
