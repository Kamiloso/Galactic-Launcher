using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GalacticLauncher.Frontend.ViewModels.Panels;

namespace GalacticLauncher.Frontend.Views.PanelViews;

internal partial class ManageView : UserControl
{
    public ManageView()
    {
        InitializeComponent();
    }
    public ManageView(AdminViewModel adminViewModel)
    {
        InitializeComponent();

        DataContext = adminViewModel;
    }
}