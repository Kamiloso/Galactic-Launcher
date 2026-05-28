using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Core.Dto;

public record GameDataUpdate
{
    public required string Token { get; init; }
    public required GameData GameData { get; init; }

    public void Deconstruct(out string token, out GameData gameData)
    {
        token = Token;
        gameData = GameData;
    }
}
