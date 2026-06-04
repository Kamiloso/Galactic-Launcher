using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GalacticLauncher.Frontend.ViewModels.Panels;

namespace GalacticLauncher.Frontend.Views.PanelViews;

internal partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }
    public HomeView(HomeViewModel homeViewModel)
    {
        InitializeComponent();

        DataContext = homeViewModel;
    }
}