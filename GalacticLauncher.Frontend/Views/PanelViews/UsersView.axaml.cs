using Avalonia;
using Avalonia.Controls;
using GalacticLauncher.Frontend.ViewModels.AdminPanels;

namespace GalacticLauncher.Frontend.Views.PanelViews;

internal partial class UsersView : UserControl
{
    public UsersView()
    {
        InitializeComponent();
    }
    public UsersView(AdUsersViewModel adUsersViewModel)
    {
        InitializeComponent();

        DataContext = adUsersViewModel;
    }
}