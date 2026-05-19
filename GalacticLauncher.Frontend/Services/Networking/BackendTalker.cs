using GalacticLauncher.Core.Dto;
using GalacticLauncher.Core.Models;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Services.Networking;

public interface IBackendTalker
{
    // EP: testing
    Task<Game> GetGameEcho(Game game);

    // EP: download
    Task<Game[]> GetAllGames();
    Task<GameData> GetGameData(long id);
    Task<Tag[]> GetAllTags();
    Task<Game[]> GetGamesByTags(long[] tagIds);

    // EP: admin
    Task<LoginResult> GetAdminToken(LoginRequest loginRequest);
}

internal class BackendTalker(IHttpService httpService) : IBackendTalker
{
    // EP: testing
    public async Task<Game> GetGameEcho(Game game) =>
        await httpService.PostAsync<Game, Game>("testing/game-echo", game);

    // EP: download
    public async Task<Game[]> GetAllGames() =>
        await httpService.GetAsync<Game[]>("download/all-games");

    public async Task<GameData> GetGameData(long id) =>
        await httpService.GetAsync<GameData>($"download/game-data?id={id}");

    public async Task<Tag[]> GetAllTags() =>
        await httpService.GetAsync<Tag[]>("download/all-tags");

    public async Task<Game[]> GetGamesByTags(long[] tagIds) =>
        await httpService.PostAsync<long[], Game[]>($"download/games-by-tags", tagIds);

    // EP: admin
    public async Task<LoginResult> GetAdminToken(LoginRequest loginRequest) =>
        await httpService.PostAsync<LoginRequest, LoginResult>("admin/req-admin", loginRequest);
}
