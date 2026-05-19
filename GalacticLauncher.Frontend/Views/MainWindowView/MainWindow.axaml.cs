using System;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using GalacticLauncher.Frontend.ViewModels.Windows;
using System.Diagnostics;

namespace GalacticLauncher.Frontend.Views.MainWindowView;

internal partial class MainWindow : Window
{
    private bool _isGalaxyTheme = true;

    public MainWindow(MainWindowViewModel mainWindowViewModel)
    {
        InitializeComponent();

        DataContext = mainWindowViewModel;

        // TODO

        // ----- MAGDA -----
        // Przenieś logikę kolorów możliwie ile się da do osobnej klasy.
        // Możesz to oprzeć na eventach w jakimś serwisie ThemeManager, który
        // będzie emitował eventy o zmianie motywu, a MainWindow będzie
        // się na nie subskrybował i zmieniał motyw.

        // Generalnie inspiruj się klasą Navigator.

        // A skoro metoda poniżej jest statyczna, to zastanów się,
        // czy ona w ogóle powinna się tu znajdować i czy nie można jej
        // gdzieś przenieść (choćby do ThemeManagera, o którym wspomniałem wyżej).

        // Za logikę ToggleTheme też powinien odpowiadać ThemeManager.
        // Okno powinno być możliwie głupie.
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