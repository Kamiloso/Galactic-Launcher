using MySqlConnector;
using System.Data;

namespace GalacticLauncher.Backend.Infrastructure.DbScopes;

public interface IAppScope : IAsyncDisposable
{
    T GetService<T>() where T : notnull;
    Task ConnectAsync(IsolationLevel? isolation);
    Task CommitAsync();
}

internal class AppScope(IServiceScope scope, DbSession session) : IAppScope
{
    private MySqlConnection Connection => session.Connection;
    private MySqlTransaction? Transaction
    {
        get => session.Transaction;
        set => session.Transaction = value;
    }

    private bool _started = false;
    private bool _disposed = false;

    public T GetService<T>() where T : notnull =>
        scope.ServiceProvider.GetRequiredService<T>();

    public async Task ConnectAsync(IsolationLevel? isolation)
    {
        if (_started || Transaction is not null)
            throw new InvalidOperationException("Transaction already started.");

        await Connection.OpenAsync();

        if (isolation.HasValue)
        {
            Transaction = await Connection.BeginTransactionAsync(isolation.Value);
        }

        _started = true;
    }

    public async Task CommitAsync()
    {
        if (Transaction is null)
            throw new InvalidOperationException("No transaction to commit.");

        await Transaction.CommitAsync();
        await Transaction.DisposeAsync();

        Transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (Transaction is not null)
        {
            try
            {
                await Transaction.RollbackAsync();
            }
            catch { /* ignore rollback errors */ }
            finally
            {
                await Transaction.DisposeAsync();

                Transaction = null;
            }
        }

        // Will also dispose the session,
        // which will dispose the connection

        if (scope is IAsyncDisposable asyncScope)
            await asyncScope.DisposeAsync();
        else
            scope.Dispose();
    }
}
