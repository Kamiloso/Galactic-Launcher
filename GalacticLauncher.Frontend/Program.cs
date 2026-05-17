using Avalonia;
using System;

namespace GalacticLauncher.Frontend;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .StartWithClassicDesktopLifetime(args);
    }
}
