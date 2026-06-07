using Avalonia.Controls;
using GalacticLauncher.Frontend.ViewModels.Panels;

namespace GalacticLauncher.Frontend.Views.PanelViews;

internal partial class LibraryView : UserControl
{
    public LibraryView()
    {
        InitializeComponent();
    }

    public LibraryView(LibraryViewModel libraryViewModel)
    {
        InitializeComponent();

        DataContext = libraryViewModel;
    }
}