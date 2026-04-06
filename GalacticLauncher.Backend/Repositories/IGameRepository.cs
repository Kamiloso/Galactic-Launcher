using GalacticLauncher.Core.DbRecords;

namespace GalacticLauncher.Backend.Repositories;

public interface IGameRepository
{
    Task<IEnumerable<GameInfo>> GetAllGames();
    Task<IEnumerable<GameInfo>> GetGamesByTagId(long tagId);
}
