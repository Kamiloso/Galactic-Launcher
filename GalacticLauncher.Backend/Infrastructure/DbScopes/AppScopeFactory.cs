using System.Data;

namespace GalacticLauncher.Backend.Infrastructure.DbScopes;

public interface IAppScopeFactory
{
    Task<IAppScope> CreateScopeNoDbAsync();
    Task<IAppScope> CreateScopeAsync(IsolationLevel? isolation);
}

internal class AppScopeFactory(
    IServiceScopeFactory scopeFactory) : IAppScopeFactory
{
    public async Task<IAppScope> CreateScopeNoDbAsync()
    {
        IServiceScope scope = scopeFactory.CreateScope();

        try
        {
            DbSession session =
                scope.ServiceProvider.GetRequiredService<DbSession>();

            return new AppScope(scope, session);
        }
        catch
        {
            if (scope is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else
                scope.Dispose();

            throw;
        }
    }

    public async Task<IAppScope> CreateScopeAsync(IsolationLevel? isolation)
    {
        IAppScope appScope = await CreateScopeNoDbAsync();

        try
        {
            await appScope.ConnectAsync(isolation);

            return appScope;
        }
        catch
        {
            await appScope.DisposeAsync();

            throw;
        }
    }
}
