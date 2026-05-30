using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using GalacticLauncher.Frontend.Infrastructure;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

public interface IThemeManager
{
    void ToggleTheme();
    void SetTheme(bool isGalaxyTheme);
}

internal class ThemeManager : IThemeManager
{
    private bool _isGalaxyTheme = true;

    public void ToggleTheme()
    {
        SetTheme(!_isGalaxyTheme);
    }

    public void SetTheme(bool isGalaxyTheme)
    {
        _isGalaxyTheme = isGalaxyTheme;

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
            DebugBox.Show(ex.ToString(), "Theme Load Error");
        }
    }
}
