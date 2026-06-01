using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Backend.Domain.Models.Extensions;

public static class ToEntityConverters
{
    public static (
        GameEntity Game,
        IEnumerable<VersionEntity> Versions,
        IEnumerable<ImageEntity> Images,
        IEnumerable<TagEntity> Tags
        ) ToEntity(this GameData game)
    {
        return (
            Game: ((Game)game).ToEntity(),
            Versions: [.. game.Versions.Select(v => v.ToEntity(game.Id))],
            Images: [.. game.Images.Select(i => i.ToEntity(game.Id))],
            Tags: [.. game.Tags.Select(t => t.ToEntity())]);
    }

    public static GameEntity ToEntity(this Game game)
    {
        return new GameEntity
        {
            Id = game.Id,
            Name = game.Name,
            Description = game.Description,
            Author = game.Author,
        };
    }

    public static VersionEntity ToEntity(this Version version, long idGame)
    {
        return new VersionEntity
        {
            Id = version.Id,
            Caption = version.Caption,
            Type = version.Type,
            Description = version.Description,
            CliArgs = version.CliArgs,
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
        return new ImageEntity
        {
            Id = image.Id,
            DownloadUrl = image.DownloadUrl,
            Type = image.Type,
            SortIndex = image.SortIndex,
            IdGame = idGame,
        };
    }

    public static TagEntity ToEntity(this Tag tag)
    {
        return new TagEntity
        {
            Id = tag.Id,
            Name = tag.Name,
            Description = tag.Description,
        };
    }

    public static HistoryEntity ToEntity(this History history)
    {
        return new HistoryEntity
        {
            Id = history.Id,
            Info = history.Info,
            Timestamp = history.Timestamp,
            IdGame = history.IdGame,
        };
    }
}
