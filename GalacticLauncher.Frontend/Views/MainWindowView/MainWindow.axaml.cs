using System;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using GalacticLauncher.Frontend.ViewModels.MainWindowViewModels;
using System.Diagnostics;

namespace GalacticLauncher.Frontend.Views.MainWindowView;

public partial class MainWindow : Window
{
    private bool _isGalaxyTheme = true;

    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }

    public void ToggleTheme()
    {
        _isGalaxyTheme = !_isGalaxyTheme;

        string themeFile = _isGalaxyTheme
            ? "PinkThemeGradient.axaml"
            : "BlueThemeGradient.axaml";

        ChangeColorTheme(themeFile);
    }

    private static void ChangeColorTheme(string themePath)
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

            foreach (var kvp in mergedDicts)
            {
                if (kvp is not ResourceInclude res) break;
                if (res.Source?.ToString().Contains("Theme") == true)
                {
                    mergedDicts.Remove(kvp);
                    break;
                }
            }

            mergedDicts.Add(newTheme);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Theme loading error: {ex.Message}");
        }
    }
}