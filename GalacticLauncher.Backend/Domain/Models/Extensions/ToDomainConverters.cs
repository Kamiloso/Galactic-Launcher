using GalacticLauncher.Core;
using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Backend.Domain.Models.Extensions;

internal static class ToDomainConverters
{
    public static Game ToDomain(this GamePlusEntity game)
    {
        return new Game
        {
            Id = game.Id,
            Name = game.Name,
            Author = game.Author,
            Description = game.Description,
            IconUrl = game.IconUrl,
            TagIdList = game.TagIdList,
        };
    }

    public static GameData ToDomain(this GamePlusEntity game,
        IEnumerable<VersionEntity> versions,
        IEnumerable<ImageEntity> images,
        IEnumerable<TagEntity> tags)
    {
        return new GameData
        {
            Id = game.Id,
            Name = game.Name,
            Author = game.Author,
            Description = game.Description,
            IconUrl = game.IconUrl,
            TagIdList = game.TagIdList,
            Versions = [.. versions.Select(ToDomain)],
            Images = [.. images.Select(ToDomain)],
            Tags = [..  tags.Select(ToDomain)],
        };
    }

    public static Version ToDomain(this VersionEntity version)
    {
        return new Version
        {
            Id = version.Id,
            Caption = version.Caption,
            Type = StringToEnum<VersionType>(version.Type),
            Description = version.Description,
            CliArgs = version.CliArgs,
            IsPrimary = version.IsPrimary,
            ReleaseDate = version.ReleaseDate,
            Platform = StringToEnum<Platform>(version.Platform),
            DownloadUrl = version.DownloadUrl,
            ExecLocation = version.ExecLocation,
            Sha256Hash = version.Sha256Hash,
            Alert = StringToEnum<AlertLevel>(version.Alert),
        };
    }

    public static Image ToDomain(this ImageEntity image)
    {
        return new Image
        {
            Id = image.Id,
            DownloadUrl = image.DownloadUrl,
            Type = StringToEnum<ImageType>(image.Type),
            SortIndex = image.SortIndex,
        };
    }

    public static Tag ToDomain(this TagEntity tag)
    {
        return new Tag
        {
            Id = tag.Id,
            Name = tag.Name,
            Description = tag.Description,
        };
    }

    public static History ToDomain(this HistoryEntity history)
    {
        return new History
        {
            Id = history.Id,
            Info = history.Info,
            Timestamp = history.Timestamp,
            IdGame = history.IdGame,
        };
    }

    private static T StringToEnum<T>(string str) where T : struct, Enum
    {
        return Enum.TryParse(str, ignoreCase: true, out T result)
            ? result
            : default;
    }
}
