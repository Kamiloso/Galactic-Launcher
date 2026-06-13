using System.Data;
using NSubstitute;
using GalacticLauncher.Backend.Infrastructure.DbScopes;
using GalacticLauncher.Backend.Tests.Helpers;

namespace GalacticLauncher.Backend.Tests.Services;

public abstract class ServiceTestBase : IAsyncDisposable
{
    protected readonly IAppScopeFactory _scopeFactory;
    protected readonly IAppScope _scope;
    protected readonly AppConfig _fakeConfig;

    protected ServiceTestBase()
    {
        _scopeFactory = Substitute.For<IAppScopeFactory>();
        _scope = Substitute.For<IAppScope>();
        _fakeConfig = TestDataHelper.CreateDummyConfig();

        _scopeFactory.CreateScopeAsync(Arg.Any<IsolationLevel?>()).Returns(_scope);
    }

    public async ValueTask DisposeAsync()
    {
        await _scope.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}