using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Backend.Domain.Models.Extensions;

public static class ToEntityConverters
{
    public static (
        GameEntity Game,
        IEnumerable<VersionEntity> Versions,
        IEnumerable<ImageEntity> Images,
        IEnumerable<long> TagIds
        ) ToEntityDeconstruct(this GameTree gameTree)
    {
        return (
            Game: gameTree.ToEntity(),
            Versions: [.. gameTree.Versions.Select(v => v.ToEntity(gameTree.Id))],
            Images: [.. gameTree.Images.Select(i => i.ToEntity(gameTree.Id))],
            TagIds: [.. gameTree.TagIds]);
    }

    public static GameEntity ToEntity(this GameRaw gameRaw)
    {
        return new GameEntity
        {
            Id = gameRaw.Id,
            Name = gameRaw.Name,
            Author = gameRaw.Author,
            Description = gameRaw.Description,
        };
    }

    public static VersionEntity ToEntity(this Version version, long idGame)
    {
        return new VersionEntity
        {
            Id = version.Id,
            Caption = version.Caption,
            Type = EnumToString(version.Type),
            Description = version.Description,
            CliArgs = version.CliArgs,
            IsPrimary = version.IsPrimary,
            ReleaseDate = version.ReleaseDate,
            Platform = EnumToString(version.Platform),
            DownloadUrl = version.DownloadUrl,
            ExecLocation = version.ExecLocation,
            Sha256Hash = version.Sha256Hash,
            Alert = EnumToString(version.Alert),
            IdGame = idGame,
        };
    }

    public static ImageEntity ToEntity(this Image image, long idGame)
    {
        return new ImageEntity
        {
            Id = image.Id,
            DownloadUrl = image.DownloadUrl,
            Type = EnumToString(image.Type),
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

    private static string EnumToString<T>(T value) where T : struct, Enum
    {
        if (!Enum.IsDefined(typeof(T), value))
            value = default;

        return value.ToString().ToLowerInvariant();
    }
}
