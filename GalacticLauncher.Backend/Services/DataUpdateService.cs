using GalacticLauncher.Backend.Domain.Exceptions;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Domain.Models.Extensions;
using GalacticLauncher.Backend.Infrastructure.DbScopes;
using GalacticLauncher.Backend.Repositories;
using GalacticLauncher.Core.Models;
using System.Data;

namespace GalacticLauncher.Backend.Services;

public interface IDataUpdateService
{
    Task UpdateGameTree(GameTree gameData);
    Task<long> CreateGame(GameRaw gameRaw);
    Task DeleteGameById(long idGame);
    Task<long> CreateTag(Tag tag);
    Task DeleteTagById(long idTag);
}

internal class DataUpdateService(
    IAppScopeFactory scopeFactory) : IDataUpdateService
{
    public async Task UpdateGameTree(GameTree gameData)
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(IsolationLevel.RepeatableRead);
        
        var gameTreeWriter = scope.GetService<IGameTreeWriter>();

        var (gameEntity, versionEntities, imageEntities, tagIdEntities) =
            gameData.ToEntityDeconstruct();

        if (!await gameTreeWriter.ReplaceGameData(
            gameEntity, versionEntities, imageEntities, tagIdEntities))
        {
            throw ClientFaultException.BadRequest400("Failed to update game information.");
        }

        await scope.CommitAsync();
    }

    public async Task<long> CreateGame(GameRaw gameRaw)
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(IsolationLevel.RepeatableRead);

        var gameRepository = scope.GetService<IGameRepository>();

        GameEntity gameEntity = gameRaw.ToEntity();

        long id = await gameRepository.CreateGame(gameEntity);
        await scope.CommitAsync();

        return id;
    }

    public async Task DeleteGameById(long idGame)
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(IsolationLevel.RepeatableRead);

        var gameRepository = scope.GetService<IGameRepository>();

        await gameRepository.DeleteGameById(idGame);
        await scope.CommitAsync();
    }

    public async Task<long> CreateTag(Tag tag)
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(IsolationLevel.RepeatableRead);

        var tagRepository = scope.GetService<ITagRepository>();

        TagEntity tagEntity = tag.ToEntity();

        long id = await tagRepository.CreateTag(tagEntity);
        await scope.CommitAsync();

        return id;
    }

    public async Task DeleteTagById(long idTag)
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(IsolationLevel.RepeatableRead);

        var tagRepository = scope.GetService<ITagRepository>();

        await tagRepository.DeleteTagById(idTag);
        await scope.CommitAsync();
    }
}
