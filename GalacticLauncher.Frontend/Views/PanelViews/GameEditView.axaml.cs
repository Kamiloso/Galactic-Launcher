using Avalonia;
using Avalonia.Controls;
using GalacticLauncher.Frontend.ViewModels.AdminPanels;
using GalacticLauncher.Frontend.ViewModels.Panels;

namespace GalacticLauncher.Frontend.Views.PanelViews;

internal partial class GameEditView : Window
{
    public GameEditView()
    {
        InitializeComponent();
    }
    public GameEditView(AdSingleGameViewModel adSingleGameViewModel)
    {
        InitializeComponent();

        DataContext = adSingleGameViewModel;
    }
}