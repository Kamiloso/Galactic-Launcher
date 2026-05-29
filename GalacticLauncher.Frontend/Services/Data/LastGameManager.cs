using GalacticLauncher.Core;
using GalacticLauncher.Frontend.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace GalacticLauncher.Frontend.Services.Data;

public interface ILastGameManager
{
    long? GetLastGame();
    void SetLastGame(long? gameId);
}

internal class LastGameManager(
    IDataRepository dataRepository) : ILastGameManager
{
    public long? GetLastGame()
    {
        List<long?> list = [.. dataRepository.GetAll(Const.KEY_LST)];

        return list.FirstOrDefault();
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
