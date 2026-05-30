using Avalonia.Controls;
using GalacticLauncher.Frontend.ViewModels;

namespace GalacticLauncher.Frontend.Views.MainWindowView;

internal partial class LoadingScreen : UserControl
{
    public LoadingScreen(LoadingViewModel load)
    {
        InitializeComponent();
        DataContext = load;
    }
}