using Avalonia.Controls;
using GalacticLauncher.Frontend.ViewModels.Windows;

namespace GalacticLauncher.Frontend.Views.MainWindowView;

internal partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel mainWindowViewModel)
    {
        InitializeComponent();

        DataContext = mainWindowViewModel;
    }
}