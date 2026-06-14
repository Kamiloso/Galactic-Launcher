using GalacticLauncher.Frontend.Repositories;
using System.Linq;

namespace GalacticLauncher.Frontend.Services.Data;

public interface ILastGameManager
{
    long? GetLastGame();
    void SetLastGame(long? gameId);
}

public class LastGameManager(
    ICacheRepository cacheRepository,
    IDataRepository dataRepository) : ILastGameManager
{
    private const string CKEY_LAST = "last";

    public long? GetLastGame()
    {
        long? id = dataRepository.GetAll(CKEY_LAST)
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
        dataRepository.Clear(CKEY_LAST);

        if (gameId.HasValue)
        {
            dataRepository.Add(CKEY_LAST, gameId.Value);
        }
    }
}
