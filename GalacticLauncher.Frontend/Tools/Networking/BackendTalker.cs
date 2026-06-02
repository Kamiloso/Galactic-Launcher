using GalacticLauncher.Core.Dto;
using GalacticLauncher.Core.Models;
using System;
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
    Task PostGameData(string token, GameData gameData);
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

    public async Task PostGameData(string token, GameData gameData) =>
        throw new NotImplementedException("Not implemented yet");
}
