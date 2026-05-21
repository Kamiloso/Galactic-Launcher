using Avalonia.Controls;
using GalacticLauncher.Frontend.ViewModels.AdminPanels;

namespace GalacticLauncher.Frontend.Views.MainPanelViews;

internal partial class TagsView : UserControl
{
    public TagsView()
    {
        InitializeComponent();
    }
    public TagsView(AdTagsViewModel adTagsViewModel)
    {
        InitializeComponent();

        DataContext = adTagsViewModel;
    }
}