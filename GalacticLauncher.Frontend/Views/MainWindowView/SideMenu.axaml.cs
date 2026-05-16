using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace GalacticLauncher.Frontend.Views.MainWindowView;

public partial class SideMenu : UserControl
{
    public SideMenu()
    {
        InitializeComponent();
    }

    public void OnSwitchThemeClick(object sender, RoutedEventArgs e)
    {
        if (VisualRoot is MainWindow mainWindow)
        {
            mainWindow.ToggleTheme();
        }
    }
}