using Avalonia.Controls;
using GalacticLauncher.Frontend.ViewModels.AdminPanels;

namespace GalacticLauncher.Frontend.Views.MainPanelViews;

internal partial class AllGamesView : UserControl
{
    public AllGamesView()
    {
        InitializeComponent();
    }
    public AllGamesView(AdGamesViewModel adGamesViewModel)
    {
        InitializeComponent();

        DataContext = adGamesViewModel;
    }
}