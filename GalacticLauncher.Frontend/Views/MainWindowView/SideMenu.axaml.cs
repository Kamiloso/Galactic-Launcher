using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace GalacticLauncher.Frontend.Views.MainWindowView;

public partial class SideMenu : UserControl
{
    private readonly int sideBarWidth = 220;
    private readonly DispatcherTimer _closeTimer;

    public SideMenu()
    {
        InitializeComponent();

        _closeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        _closeTimer.Tick += (s, e) =>
        {
            Width = 0;
            _closeTimer.Stop();
        };

        this.PointerEntered += (s, e) =>
        {
            _closeTimer.Stop();
            Width = sideBarWidth;
        };

        this.PointerExited += (s, e) =>
        {
            _closeTimer.Stop();
            _closeTimer.Start();
        };
    }

    public void ForceOpen()
    {
        Width = sideBarWidth;

        _closeTimer.Stop();
        _closeTimer.Start();
    }
}