using System;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using GalacticLauncher.Frontend.Network;
using GalacticLauncher.Frontend.ViewModels.MainWindowViewModels;

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


    private bool _isGalaxyTheme = true;

    public void ToggleTheme()
    {
        _isGalaxyTheme = !_isGalaxyTheme;
        string themeFile = _isGalaxyTheme ? "PinkThemeGradient.axaml" : "BlueThemeGradient.axaml";

        ChangeColorTheme(themeFile);
    }

    private void ChangeColorTheme(string themePath)
    {
        if (Application.Current?.Resources is not { } resources)
            return;

        var mergedDicts = resources.MergedDictionaries;

        try
        {
            var themeUri = new Uri($"avares://GalacticLauncher.Frontend/AvaloniaResources/ResourceDictionaries/{themePath}");
            var newTheme = new ResourceInclude(themeUri)
            {
                Source = themeUri
            };

            for (int i = 0; i < mergedDicts.Count; i++)
            {
                if (mergedDicts[i] is ResourceInclude res && res.Source?.ToString().Contains("Theme") == true)
                {
                    mergedDicts.RemoveAt(i);
                    break;
                }
            }
            mergedDicts.Add(newTheme);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"B³¹d ³adowania motywu: {ex.Message}");
        }
    }
}