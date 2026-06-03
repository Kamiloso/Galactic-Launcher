using GalacticLauncher.Core.Dto;
using GalacticLauncher.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Tools.Networking;

public interface IBackendTalker
{
    // EP: testing
    Task<Game> GetGameEcho(Game game);

    // EP: download
    Task<IEnumerable<Game>> GetAllGames();
    Task<GameData> GetGameData(long id);
    Task<IEnumerable<Tag>> GetAllTags();

    // EP: admin
    Task<LoginResult> GetAdminToken(LoginRequest loginRequest);
    Task PostGameTree(string token, GameTree gameTree);
    Task CreateGame(string token, Game game);
    Task DeleteGame(string token, long id);
    Task CreateTag(string token, Tag tag);
    Task DeleteTag(string token, long id);
    Task<IEnumerable<History>> GetHistoryPage(string token, int page);
}

internal class BackendTalker(IHttpPoster httpPoster) : IBackendTalker
{
    // EP: testing
    public async Task<Game> GetGameEcho(Game game) =>
        await httpPoster.PostAsync<Game, Game>("testing/game-echo", game);

    // EP: download
    public async Task<IEnumerable<Game>> GetAllGames() =>
        await httpPoster.GetAsync<IEnumerable<Game>>("download/all-games");

    public async Task<GameData> GetGameData(long id) =>
        await httpPoster.GetAsync<GameData>($"download/game-data?id={id}");

    public async Task<IEnumerable<Tag>> GetAllTags() =>
        await httpPoster.GetAsync<IEnumerable<Tag>>("download/all-tags");

    // EP: admin
    public async Task<LoginResult> GetAdminToken(LoginRequest loginRequest) =>
        await httpPoster.PostAsync<LoginRequest, LoginResult>("admin/req-admin", loginRequest);

    public async Task PostGameTree(string token, GameTree gameTree) =>
        await httpPoster.PostAsync("admin/post-game-tree",
            new AdminBox<GameTree>() { Token = token, Body = gameTree });

    public async Task CreateGame(string token, Game game) =>
        await httpPoster.PostAsync("admin/create-game",
            new AdminBox<Game>() { Token = token, Body = game });

    public async Task DeleteGame(string token, long id) =>
        await httpPoster.PostAsync("admin/delete-game",
            new AdminBox<long>() { Token = token, Body = id });

    public async Task CreateTag(string token, Tag tag) =>
        await httpPoster.PostAsync("admin/create-tag",
            new AdminBox<Tag>() { Token = token, Body = tag });

    public async Task DeleteTag(string token, long id) =>
        await httpPoster.PostAsync("admin/delete-tag",
            new AdminBox<long>() { Token = token, Body = id });

    public async Task<IEnumerable<History>> GetHistoryPage(string token, int page) =>
        await httpPoster.PostAsync<AdminBox, IEnumerable<History>>($"admin/get-history-page?page={page}",
            new AdminBox() { Token = token });
}
