using GalacticLauncher.Core;
using GalacticLauncher.Frontend.Repositories;
using System.Linq;

namespace GalacticLauncher.Frontend.Services.Data;

public interface ILastGameManager
{
    long? GetLastGame();
    void SetLastGame(long? gameId);
}

internal class LastGameManager(
    ICacheRepository cacheRepository,
    IDataRepository dataRepository) : ILastGameManager
{
    public long? GetLastGame()
    {
        long? id = dataRepository.GetAll(Const.KEY_LST)
            .Select(id => (long?)id)
            .FirstOrDefault();

        if (id.HasValue && cacheRepository.GetGame(id.Value) != null)
        {
            return id.Value;
        }

        return null;
    }

    public void SetLastGame(long? gameId)
    {
        dataRepository.Clear(Const.KEY_LST);

        if (gameId.HasValue)
        {
            dataRepository.Add(Const.KEY_LST, gameId.Value);
        }
    }
}
