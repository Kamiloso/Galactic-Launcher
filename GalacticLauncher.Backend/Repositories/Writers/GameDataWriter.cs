using Dapper;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Infrastructure;
using GalacticLauncher.Backend.Domain.Exceptions;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories.Writers;

public interface IGameDataWriter
{
    Task ReplaceGameData(
        GameEntity game,
        IEnumerable<VersionEntity> versions,
        IEnumerable<ImageEntity> images,
        IEnumerable<TagEntity> tags);
}

internal class GameDataWriter(DbSession session) : IGameDataWriter
{
    private const int ERR_FOREIGN_KEY_VIOLATION = 1452;

    private readonly MySqlConnection _db = session.Connection;

    /// <exception cref="DataIntegrityException"></exception>
    public async Task ReplaceGameData(
        GameEntity game,
        IEnumerable<VersionEntity> versions,
        IEnumerable<ImageEntity> images,
        IEnumerable<TagEntity> tags)
    {
        long idGame = game.Id;

        if (!await TryAcquireXLockOnExistingGame(idGame))
            throw new DataIntegrityException($"Game with id {idGame} does not exist.");

        await SyncGame(idGame, game);
        await SyncVersions(idGame, versions);
        await SyncImages(idGame, images);
        await SyncTags(idGame, tags);
    }

    private async Task<bool> TryAcquireXLockOnExistingGame(long idGame)
    {
        long? lockId = await _db.QueryFirstOrDefaultAsync<long?>(
            "SELECT id FROM games WHERE id = @p1 FOR UPDATE",
            new { p1 = idGame },
            transaction: session.Transaction);

        return lockId.HasValue;
    }

    private async Task SyncGame(long idGame, GameEntity game)
    {
        game = game with { Id = idGame };

        await _db.ExecuteAsync("""
            UPDATE games SET
                name = @name,
                author = @author,
                description = @description
            WHERE id = @id
            """,
            new
            {
                id = game.Id,
                name = game.Name,
                author = game.Author,
                description = game.Description,
            },
            transaction: session.Transaction);
    }

    private async Task SyncVersions(long idGame, IEnumerable<VersionEntity> versions)
    {
        versions = [.. versions.Select(v => v with { IdGame = idGame })];

        // Obtain existing versions

        List<long> allIds = [..
            await _db.QueryAsync<long>(
                "SELECT id FROM versions WHERE id_game = @p1",
                new { p1 = idGame },
                transaction: session.Transaction)
            ];

        // Modify existing versions

        List<VersionEntity> toModify = [.. versions
            .Where(v => allIds.Contains(v.Id))];

        if (toModify.Count > 0)
        {
            await _db.ExecuteAsync("""
            UPDATE versions SET
                caption = @Caption,
                type = @Type,
                description = @Description,
                cli_args = @CliArgs,
                is_primary = @IsPrimary,
                release_date = @ReleaseDate,
                platform = @Platform,
                download_url = @DownloadUrl,
                exec_location = @ExecLocation,
                sha256_hash = @Sha256Hash,
                alert = @Alert
            WHERE id = @Id
            """,
            toModify,
            transaction: session.Transaction);
        }

        // Add new versions

        List<VersionEntity> toAdd = [.. versions
            .Where(v => !allIds.Contains(v.Id))];

        if (toAdd.Count > 0)
        {
            await _db.ExecuteAsync("""
                INSERT INTO versions
                    (caption, type, description, cli_args, is_primary, release_date,
                    platform, download_url, exec_location, sha256_hash, alert, id_game)
                VALUES
                    (@Caption, @Type, @Description, @CliArgs, @IsPrimary, @ReleaseDate,
                    @Platform, @DownloadUrl, @ExecLocation, @Sha256Hash, @Alert, @IdGame)
                """,
                toAdd,
                transaction: session.Transaction);
        }

        // Delete old versions

        List<object> toDeleteIds = [.. allIds
            .Where(id => !versions.Any(v => v.Id == id))
            .Select(id => new { Id = id })];

        if (toDeleteIds.Count > 0)
        {
            await _db.ExecuteAsync(
                "DELETE FROM versions WHERE id = @Id",
                toDeleteIds,
                transaction: session.Transaction);
        }
    }

    private async Task SyncImages(long idGame, IEnumerable<ImageEntity> images)
    {
        images = [.. images.Select(i => i with { IdGame = idGame })];

        // Obtain existing images

        List<long> allIds = [..
            await _db.QueryAsync<long>(
                "SELECT id FROM images WHERE id_game = @p1",
                new { p1 = idGame },
                transaction: session.Transaction)
            ];

        // Modify existing images

        List<ImageEntity> toModify = [.. images
            .Where(i => allIds.Contains(i.Id))];

        if (toModify.Count > 0)
        {
            await _db.ExecuteAsync("""
                UPDATE images SET
                    download_url = @DownloadUrl,
                    type = @Type,
                    sort_index = @SortIndex
                WHERE id = @Id
                """,
                toModify,
                transaction: session.Transaction);
        }

        // Add new images

        List<ImageEntity> toAdd = [.. images
            .Where(i => !allIds.Contains(i.Id))];

        if (toAdd.Count > 0)
        {
            await _db.ExecuteAsync("""
                INSERT INTO images
                    (download_url, type, sort_index, id_game)
                VALUES
                    (@DownloadUrl, @Type, @SortIndex, @IdGame)
                """,
                toAdd,
                transaction: session.Transaction);
        }

        // Delete old images

        List<object> toDeleteIds = [.. allIds
            .Where(id => !images.Any(i => i.Id == id))
            .Select(id => new { Id = id })];

        if (toDeleteIds.Count > 0)
        {
            await _db.ExecuteAsync(
                "DELETE FROM images WHERE id = @Id",
                toDeleteIds,
                transaction: session.Transaction);
        }
    }

    private async Task SyncTags(long idGame, IEnumerable<TagEntity> tags)
    {
        List<long> targetIds = [.. tags
            .Select(t => t.Id)];

        List<long> existingIds = [..
            await _db.QueryAsync<long>(
                "SELECT id_tag FROM games_tags WHERE id_game = @p1",
                new { p1 = idGame },
                transaction: session.Transaction)
            ];

        try
        {
            // Delete old tags

            List<long> toDeleteIds = [.. existingIds
                .Where(id => !targetIds.Contains(id))];

            if (toDeleteIds.Count > 0)
            {
                await _db.ExecuteAsync("""
                    DELETE FROM games_tags
                    WHERE id_game = @IdGame AND id_tag IN @ToDeleteIds
                    """,
                    new { IdGame = idGame, ToDeleteIds = toDeleteIds },
                    transaction: session.Transaction);
            }

            // Add new tags

            List<object> toAdd = [.. targetIds
                .Where(id => !existingIds.Contains(id))
                .Select(id => new { IdGame = idGame, IdTag = id })];

            if (toAdd.Count > 0)
            {
                await _db.ExecuteAsync("""
                    INSERT INTO games_tags (id_game, id_tag)
                    VALUES (@IdGame, @IdTag)
                    """,
                    toAdd,
                    transaction: session.Transaction);
            }
        }
        catch (MySqlException ex) when (ex.Number == ERR_FOREIGN_KEY_VIOLATION)
        {
            throw new DataIntegrityException("One or more tags do not exist.", ex);
        }
    }
}
