using System.Data;

namespace GalacticLauncher.Backend.Infrastructure.DbScopes;

public interface IAppScopeFactory
{
    Task<IAppScope> CreateScopeAsync(IsolationLevel? isolationLevel);
}

internal class AppScopeFactory(IServiceScopeFactory scopeFactory) : IAppScopeFactory
{
    public async Task<IAppScope> CreateScopeAsync(IsolationLevel? isolationLevel)
    {
        var scope = scopeFactory.CreateScope();
        var session = scope.ServiceProvider.GetRequiredService<DbSession>();

        try
        {
            await session.Connection.OpenAsync();

            if (isolationLevel.HasValue)
            {
                session.Transaction =
                    await session.Connection.BeginTransactionAsync(isolationLevel.Value);
            }

            return new AppScope(scope, session);
        }
        catch
        {
            await scope.DisposeSuitable();

            throw;
        }
    }
}
