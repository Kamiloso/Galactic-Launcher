using Dapper;
using GalacticLauncher.Backend.Domain.Models;
using GalacticLauncher.Backend.Infrastructure;
using MySqlConnector;

namespace GalacticLauncher.Backend.Repositories;

public interface IHistoryRepository
{
    Task AddLog(HistoryEntity history);
    Task<IEnumerable<HistoryEntity>> GetHistoryEntries(int page, int pageSize);
    Task ReduceHistoryTo(int amount);
}

internal class HistoryRepository(DbSession session) : IHistoryRepository
{
    private readonly MySqlConnection _db = session.Connection;

    public async Task AddLog(HistoryEntity history)
    {
        await _db.ExecuteAsync("""
            INSERT INTO history
                (info, timestamp, id_game) VALUES
                (@Info, @Timestamp, (SELECT id FROM games WHERE id = @IdGame))
            """,
            history,
            transaction: session.Transaction);
    }

    public async Task<IEnumerable<HistoryEntity>> GetHistoryEntries(int page, int pageSize)
    {
        long lpage = Math.Max(0, page);

        return await _db.QueryAsync<HistoryEntity>("""
            SELECT * FROM history
                ORDER BY id DESC
                LIMIT @Limit OFFSET @Offset
            """,
            new { Limit = pageSize, Offset = lpage * pageSize },
            transaction: session.Transaction);
    }

    public async Task ReduceHistoryTo(int amount)
    {
        long lamount = Math.Max(0, amount);

        long? thresholdId = await _db.QueryFirstOrDefaultAsync<long?>("""
            SELECT id FROM history
                ORDER BY id DESC
                LIMIT 1 OFFSET @Offset
            """,
            new { Offset = lamount },
            transaction: session.Transaction);

        if (thresholdId is null)
            return;

        await _db.ExecuteAsync("""
            DELETE FROM history
                WHERE id <= @Threshold
            """,
            new { Threshold = thresholdId },
            transaction: session.Transaction);
    }
}
