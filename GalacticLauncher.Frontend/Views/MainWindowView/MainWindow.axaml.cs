using Avalonia.Controls;
using Avalonia.Input;
using GalacticLauncher.Frontend.Network;
using GalacticLauncher.Frontend.ViewModels.MainWindowViewModel;

namespace GalacticLauncher.Frontend.Views.MainWindowView;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel()
        {
            Api = new HttpApiService(
                HttpClientProvider.HttpClient
                )
        };
    }

    private void Trigger_PointerEntered(object? sender, PointerEventArgs e)
    {
        SideBar.ForceOpen();
    }
}