namespace GalacticLauncher.Backend.Infrastructure.DbScopes;

public interface IAppScope : IAsyncDisposable
{
    T Resolve<T>() where T : notnull;
    Task CompleteAsync();
}

internal class AppScope(IServiceScope scope, DbSession session) : IAppScope
{
    private bool _isCompleted = false;

    public T Resolve<T>() where T : notnull =>
        scope.ServiceProvider.GetRequiredService<T>();

    public async Task CompleteAsync()
    {
        if (session.Transaction is not null && !_isCompleted)
        {
            await session.Transaction.CommitAsync();
            _isCompleted = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_isCompleted && session.Transaction is not null)
        {
            try { await session.Transaction.RollbackAsync(); }
            catch { /* ignore rollback errors */ }
        }

        await session.DisposeAsync();
        await scope.DisposeSuitable();
    }
}
