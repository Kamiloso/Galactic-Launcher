using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GalacticLauncher.Frontend.ViewModels.Panels;

namespace GalacticLauncher.Frontend.Views.MainPanelViews;

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