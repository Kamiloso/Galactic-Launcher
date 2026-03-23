using Avalonia.Controls;
using GalacticLauncher.Frontend.Network;

namespace GalacticLauncher.Frontend;

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
}