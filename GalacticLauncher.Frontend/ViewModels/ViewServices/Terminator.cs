using Avalonia.Controls.ApplicationLifetimes;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

public interface ITerminator
{
    void Terminate();
}

internal class Terminator(
    IClassicDesktopStyleApplicationLifetime desktop) : ITerminator
{
    public void Terminate()
    {
        desktop.Shutdown();
    }
}
