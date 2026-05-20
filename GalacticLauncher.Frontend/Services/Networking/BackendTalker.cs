using GalacticLauncher.Core.Dto;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using System;
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
    Task PostGameData(string token, GameData gameData);
}

internal class BackendTalker(IHttpPoster httpPoster) : IBackendTalker
{
    // EP: testing
    public async Task<Game> GetGameEcho(Game game) =>
        await httpPoster.PostAsync<Game, Game>("testing/game-echo", game);

    // EP: download
    public async Task<Game[]> GetAllGames() =>
        await httpPoster.GetAsync<Game[]>("download/all-games");

    public async Task<GameData> GetGameData(long id) =>
        await httpPoster.GetAsync<GameData>($"download/game-data?id={id}");

    public async Task<Tag[]> GetAllTags() =>
        await httpPoster.GetAsync<Tag[]>("download/all-tags");

    public async Task<Game[]> GetGamesByTags(long[] tagIds) =>
        await httpPoster.PostAsync<long[], Game[]>($"download/games-by-tags", tagIds);

    // EP: admin
    public async Task<LoginResult> GetAdminToken(LoginRequest loginRequest) =>
        await httpPoster.PostAsync<LoginRequest, LoginResult>("admin/req-admin", loginRequest);

    public async Task PostGameData(string token, GameData gameData) =>
        throw new NotImplementedException("Not implemented yet");
}
