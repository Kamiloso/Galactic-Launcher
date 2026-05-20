using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Backend.Domain.Models.Conversions;

public static class ToEntityExtensions
{
    public static (
        GameEntity Game,
        IEnumerable<VersionEntity> Versions,
        IEnumerable<ImageEntity> Images,
        IEnumerable<TagEntity> Tags
        ) ToEntity(GameData game)
    {
        return (
            Game: new GameEntity()
            {
                Name = game.Name,
                Description = game.Description,
            },
            Versions: [.. game.Versions.Select(v => v.ToEntity(game.Id))],
            Images: [.. game.Images.Select(i => i.ToEntity(game.Id))],
            Tags: [.. game.Tags.Select(t => t.ToEntity())]
            );
    }

    public static VersionEntity ToEntity(this Version version, long idGame)
    {
        return new VersionEntity()
        {
            Id = version.Id,
            Caption = version.Caption,
            Type = version.Type,
            Description = version.Description,
            IsPrimary = version.IsPrimary,
            ReleaseDate = version.ReleaseDate,
            Platform = version.Platform,
            DownloadUrl = version.DownloadUrl,
            ExecLocation = version.ExecLocation,
            Sha256Hash = version.Sha256Hash,
            Alert = version.Alert,
            IdGame = idGame,
        };
    }

    public static ImageEntity ToEntity(this Image image, long idGame)
    {
        return new ImageEntity()
        {
            DownloadUrl = image.DownloadUrl,
            Type = image.Type,
            SortIndex = image.SortIndex,
            IdGame = idGame,
        };
    }

    public static TagEntity ToEntity(this Tag tag)
    {
        return new TagEntity()
        {
            Name = tag.Name,
            Description = tag.Description,
        };
    }
}
