using GalacticLauncher.Backend.Domain.Exceptions;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Domain.Models.Extensions;
using GalacticLauncher.Backend.Infrastructure.DbScopes;
using GalacticLauncher.Backend.Repositories.Writers;
using GalacticLauncher.Core.Models;
using System.Data;

namespace GalacticLauncher.Backend.Services;

public interface IDataUpdateService
{
    Task UpdateGameData(GameData gameData);
    Task<long> CreateGame(Game game);
    Task DeleteGameById(long idGame);
    Task<long> CreateTag(Tag tag);
    Task DeleteTagById(long idTag);
}

internal class DataUpdateService(
    IAppScopeFactory scopeFactory) : IDataUpdateService
{
    public async Task UpdateGameData(GameData gameData)
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(IsolationLevel.RepeatableRead);
        
        var gameDataWriter = scope.GetService<IGameDataWriter>();

        try
        {
            var tuple = gameData.ToEntity();

            GameEntity gameEntity = tuple.Game;
            IEnumerable<VersionEntity> versionEntities = tuple.Versions;
            IEnumerable<ImageEntity> imageEntities = tuple.Images;
            IEnumerable<TagEntity> tagEntities = tuple.Tags;

            await gameDataWriter.ReplaceGameData(
                gameEntity, versionEntities, imageEntities, tagEntities);
        }
        catch (DataIntegrityException ex)
        {
            throw ClientFaultException.BadRequest400(ex.Message);
        }

        await scope.CommitAsync();
    }

    public async Task<long> CreateGame(Game game)
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(IsolationLevel.RepeatableRead);

        var gameWriter = scope.GetService<IGameWriter>();

        GameEntity gameEntity = game.ToEntity();

        long id = await gameWriter.CreateGame(gameEntity);
        await scope.CommitAsync();

        return id;
    }

    public async Task DeleteGameById(long idGame)
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(IsolationLevel.RepeatableRead);

        var gameWriter = scope.GetService<IGameWriter>();

        await gameWriter.DeleteGameById(idGame);
        await scope.CommitAsync();
    }

    public async Task<long> CreateTag(Tag tag)
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(IsolationLevel.RepeatableRead);

        var tagWriter = scope.GetService<ITagWriter>();

        TagEntity tagEntity = tag.ToEntity();

        long id = await tagWriter.CreateTag(tagEntity);
        await scope.CommitAsync();

        return id;
    }

    public async Task DeleteTagById(long idTag)
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(IsolationLevel.RepeatableRead);

        var tagWriter = scope.GetService<ITagWriter>();

        await tagWriter.DeleteTagById(idTag);
        await scope.CommitAsync();
    }
}
