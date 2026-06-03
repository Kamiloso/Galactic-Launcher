namespace GalacticLauncher.Core.Models.Extensions;

public static class DomainConverters
{
    public static GameTree ToGameTree(this GameData gameData)
    {
        return new GameTree
        {
            Id = gameData.Id,
            Name = gameData.Name,
            Author = gameData.Author,
            Description = gameData.Description,
            Versions = gameData.Versions,
            Images = gameData.Images,
            TagIds = gameData.Tags.Select(t => t.Id)
        };
    }
}
