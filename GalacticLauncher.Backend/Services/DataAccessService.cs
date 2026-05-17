using GalacticLauncher.Backend.Infrastructure.DbScopes;
using GalacticLauncher.Backend.Models;
using GalacticLauncher.Backend.Models.Conversions;
using GalacticLauncher.Backend.Repositories;
using GalacticLauncher.Core.Models;
using System.Data;

namespace GalacticLauncher.Backend.Services;

public interface IDataAccessService
{
    Task<GameData> GetGameDataById(long id);
    Task<IEnumerable<Game>> GetAllGames();
    Task<IEnumerable<Tag>> GetAllTags();
    Task<IEnumerable<Game>> GetGamesByTagIds(IEnumerable<long> tagIds);
}

internal class DataAccessService(
    IAppScopeFactory scopeFactory
    ) : IDataAccessService
{
    public async Task<GameData> GetGameDataById(long id)
    {
        await using var scope = await scopeFactory.CreateScopeAsync(
            IsolationLevel.RepeatableRead);

        var gameRepository = scope.Resolve<IGameRepository>();
        var imageRepository = scope.Resolve<IImageRepository>();
        var versionRepository = scope.Resolve<IVersionRepository>();
        var tagRepository = scope.Resolve<ITagRepository>();

        GameEntity game = await gameRepository.GetGameById(id);

        return game.ToDomain(
            await versionRepository.GetVersionsByGameId(id),
            await imageRepository.GetImagesByGameId(id),
            await tagRepository.GetTagsByGameId(id)
            );
    }

    public async Task<IEnumerable<Game>> GetAllGames()
    {
        await using var scope = await scopeFactory.CreateScopeAsync(
            IsolationLevel.RepeatableRead);

        var gameRepository = scope.Resolve<IGameRepository>();

        IEnumerable<Game> games =
            (await gameRepository.GetAllGamesWithIcons())
            .Select(g => g.ToDomain());

        return games;
    }

    public async Task<IEnumerable<Tag>> GetAllTags()
    {
        await using var scope = await scopeFactory.CreateScopeAsync(
            IsolationLevel.RepeatableRead);

        var tagRepository = scope.Resolve<ITagRepository>();

        IEnumerable<Tag> tags =
            (await tagRepository.GetAllTags())
            .Select(t => t.ToDomain());

        return tags;
    }

    public async Task<IEnumerable<Game>> GetGamesByTagIds(IEnumerable<long> tagIds)
    {
        await using var scope = await scopeFactory.CreateScopeAsync(
            IsolationLevel.RepeatableRead);

        var gameRepository = scope.Resolve<IGameRepository>();

        IEnumerable<Game> games =
            (await gameRepository.GetGamesByTagIds(tagIds))
            .Select(g => g.ToDomain());

        return games;
    }
}
