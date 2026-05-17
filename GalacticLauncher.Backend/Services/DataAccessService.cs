using GalacticLauncher.Backend.Infrastructure;
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
        await using var scope =
            await scopeFactory.CreateScopeAsync(IsolationLevel.RepeatableRead);

        var gameRepository = scope.GetService<IGameRepository>();
        var imageRepository = scope.GetService<IImageRepository>();
        var versionRepository = scope.GetService<IVersionRepository>();
        var tagRepository = scope.GetService<ITagRepository>();

        GameEntity game = await gameRepository.GetGameById(id)
            ?? throw ClientFaultException.NotFound404($"Game with id {id} not found.");

        return game.ToDomain(
            await versionRepository.GetVersionsByGameId(id),
            await imageRepository.GetImagesByGameId(id),
            await tagRepository.GetTagsByGameId(id)
            );
    }

    public async Task<IEnumerable<Game>> GetAllGames()
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(isolation: null);

        var gameRepository = scope.GetService<IGameRepository>();

        IEnumerable<Game> games =
            (await gameRepository.GetAllGames())
            .Select(g => g.ToDomain());

        return games;
    }

    public async Task<IEnumerable<Tag>> GetAllTags()
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(isolation: null);

        var tagRepository = scope.GetService<ITagRepository>();

        IEnumerable<Tag> tags =
            (await tagRepository.GetAllTags())
            .Select(t => t.ToDomain());

        return tags;
    }

    public async Task<IEnumerable<Game>> GetGamesByTagIds(IEnumerable<long> tagIds)
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(isolation: null);

        var gameRepository = scope.GetService<IGameRepository>();

        IEnumerable<Game> games =
            (await gameRepository.GetAllGamesWithTagContraints(tagIds))
            .Select(g => g.ToDomain());

        return games;
    }
}
