using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Styling;
using GalacticLauncher.Frontend.ViewModels.Windows;

namespace GalacticLauncher.Frontend.Views.MainWindowView;

internal partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

    }
    public MainWindow(MainWindowViewModel mainWindowViewModel)
    {
        InitializeComponent();

        DataContext = mainWindowViewModel;
        
    }
}