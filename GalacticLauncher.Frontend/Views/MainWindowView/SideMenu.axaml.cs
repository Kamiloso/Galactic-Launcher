using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using GalacticLauncher.Frontend.ViewModels.Windows;

namespace GalacticLauncher.Frontend.Views.MainWindowView;

internal partial class SideMenu : UserControl
{
    public SideMenu()
    {
        InitializeComponent();
    }
    public SideMenu(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}