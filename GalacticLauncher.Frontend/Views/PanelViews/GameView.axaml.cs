using Avalonia.Controls;
using GalacticLauncher.Frontend.ViewModels.Panels;

namespace GalacticLauncher.Frontend.Views.PanelViews;

internal partial class GameView : UserControl
{
    public GameView()
    {
        InitializeComponent();
    }
    public GameView(GameViewModel gameViewModel)
    {
        InitializeComponent();

        DataContext = gameViewModel;
    }
}