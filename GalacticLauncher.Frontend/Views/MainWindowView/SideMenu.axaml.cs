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
    }
}