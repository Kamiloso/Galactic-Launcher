using Avalonia;
using GalacticLauncher.Frontend.Infrastructure.System;
using System;

namespace GalacticLauncher.Frontend;

internal class Program
{
    private const string MUTEX_NAME = "Frontend-Startup";

    [STAThread]
    public static void Main(string[] args)
    {
        if (SystemMutex.TryAcquire(MUTEX_NAME, 0, out var outMutex))
        {
            using SystemMutex mutex = outMutex;

            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .StartWithClassicDesktopLifetime(args);
        }
    }
}
