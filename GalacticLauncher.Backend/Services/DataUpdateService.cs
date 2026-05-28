using GalacticLauncher.Backend.Infrastructure.DbScopes;
using GalacticLauncher.Backend.Repositories;
using GalacticLauncher.Core.Models;
using System.Data;

namespace GalacticLauncher.Backend.Services;

public interface IDataUpdateService
{
    Task UpdateGameData(GameData gameData);
}

internal class DataUpdateService(
    IAppScopeFactory scopeFactory) : IDataUpdateService
{
    public async Task UpdateGameData(GameData gameDataUpdate)
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(IsolationLevel.RepeatableRead);

        var gameRepository = scope.GetService<IGameRepository>();
        var versionRepository = scope.GetService<IVersionRepository>();
        var imageRepository = scope.GetService<IImageRepository>();
        var tagRepository = scope.GetService<ITagRepository>();

        foreach (var repo in new object[]
            { gameRepository, versionRepository, imageRepository, tagRepository })
        {
            repo.LockWrite();
        }



        // TODO: Implement the logic to update
        // the game data in the database using the repositories.
    }
}
