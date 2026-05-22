using Avalonia;
using Avalonia.Controls;
using GalacticLauncher.Frontend.ViewModels.AdminPanels;
using GalacticLauncher.Frontend.ViewModels.Panels;

namespace GalacticLauncher.Frontend.Views.MainPanelViews;

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