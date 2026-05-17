namespace GalacticLauncher.Backend.Infrastructure;

public static class PatchExtensions
{
    public static async ValueTask DisposeSuitable(this object? obj)
    {
        if (obj is IAsyncDisposable asyncDisposable)
            await asyncDisposable.DisposeAsync();

        else if (obj is IDisposable disposable)
            disposable.Dispose();
    }
}
