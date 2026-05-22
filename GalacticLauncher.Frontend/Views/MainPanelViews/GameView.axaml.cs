using Avalonia.Controls;
using GalacticLauncher.Frontend.ViewModels.Panels;

namespace GalacticLauncher.Frontend.Views.MainPanelViews;

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