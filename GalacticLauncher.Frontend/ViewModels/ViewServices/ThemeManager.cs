using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

public interface IThemeManager
{
    event Action<string>? ThemeErrorOccurred;
    void ToggleTheme();
}

internal class ThemeManager : IThemeManager
{
    public event Action<string>? ThemeErrorOccurred;
    
    private bool _isGalaxyTheme = true;

    public void ToggleTheme()
    {
        _isGalaxyTheme = !_isGalaxyTheme;

        string themeFile = _isGalaxyTheme
            ? "PinkThemeGradient.axaml"
            : "BlueThemeGradient.axaml";

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
            ThemeErrorOccurred?.Invoke($"Nie udało się załadować motywu: {ex.Message}");
        }
    }
}
