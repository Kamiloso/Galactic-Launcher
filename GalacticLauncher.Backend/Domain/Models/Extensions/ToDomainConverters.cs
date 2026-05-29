using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Backend.Domain.Models.Extensions;

internal static class ToDomainConverters
{
    public static Game ToDomain(this GameEntity game, ImageEntity? image)
    {
        return new Game
        {
            Id = game.Id,
            Name = game.Name,
            Author = game.Author,
            Description = game.Description,
            IconUrl = image?.DownloadUrl,
        };
    }

    public static Game ToDomain(this GameWithIconEntity game)
    {
        return new Game
        {
            Id = game.Id,
            Name = game.Name,
            Description = game.Description,
            Author = game.Author,
            IconUrl = game.IconUrl,
        };
    }

    public static GameData ToDomain(this GameWithIconEntity game,
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
        };
    }

    public static Image ToDomain(this ImageEntity image)
    {
        return new Image
        {
            DownloadUrl = image.DownloadUrl,
            Type = image.Type,
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
}
