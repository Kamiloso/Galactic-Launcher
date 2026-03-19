using System;
using System.Net.Http;
using Avalonia.Controls;
using GalacticLauncher.Frontend.Network;
using GalacticLauncher.Frontend.Services;

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