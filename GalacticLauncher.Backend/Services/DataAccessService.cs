using GalacticLauncher.Backend.Domain.Exceptions;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Domain.Models.Extensions;
using GalacticLauncher.Backend.Infrastructure.DbScopes;
using GalacticLauncher.Backend.Repositories.Readers;
using GalacticLauncher.Core.Models;
using System.Data;

namespace GalacticLauncher.Backend.Services;

public interface IDataAccessService
{
    Task<GameData> GetGameDataById(long id);
    Task<IEnumerable<Game>> GetAllGames();
    Task<IEnumerable<Tag>> GetAllTags();
}

internal class DataAccessService(
    IAppScopeFactory scopeFactory) : IDataAccessService
{
    public async Task<GameData> GetGameDataById(long id)
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(IsolationLevel.RepeatableRead);

        var gameReader = scope.GetService<IGameReader>();
        var versionReader = scope.GetService<IVersionReader>();
        var imageReader = scope.GetService<IImageReader>();
        var tagReader = scope.GetService<ITagReader>();

        GameWithIconEntity game = await gameReader.GetGameById(id)
            ?? throw ClientFaultException.NotFound404($"Game with id {id} not found.");

        return game.ToDomain(
            await versionReader.GetVersionsByGameId(id),
            await imageReader.GetImagesByGameId(id),
            await tagReader.GetTagsByGameId(id)
            );
    }

    public async Task<IEnumerable<Game>> GetAllGames()
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(isolation: null);

        var gameReader = scope.GetService<IGameReader>();

        IEnumerable<Game> games =
            (await gameReader.GetAllGames())
            .Select(g => g.ToDomain());

        return games;
    }

    public async Task<IEnumerable<Tag>> GetAllTags()
    {
        await using var scope =
            await scopeFactory.CreateScopeAsync(isolation: null);

        var tagReader = scope.GetService<ITagReader>();

        IEnumerable<Tag> tags =
            (await tagReader.GetAllTags())
            .Select(t => t.ToDomain());

        return tags;
    }
}
