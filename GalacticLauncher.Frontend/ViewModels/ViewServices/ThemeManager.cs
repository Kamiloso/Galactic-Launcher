using System;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using GalacticLauncher.Frontend.Services.Data;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

public interface IThemeManager
{
    void ToggleTheme();
    void SetTheme(bool isGalaxyTheme);
}

internal class ThemeManager : IThemeManager
{
    private bool IsGalaxyTheme
    {
        get => _preferenceManager.IsThemeGalactic;
        set => _preferenceManager.IsThemeGalactic = value;
    }

    private readonly IPreferenceManager _preferenceManager;
    private readonly INotifications _notifications;

    public ThemeManager(
        IPreferenceManager preferenceManager,
        INotifications notifications)
    {
        _preferenceManager = preferenceManager;
        _notifications = notifications;

        SetTheme(IsGalaxyTheme);
    }

    public void ToggleTheme()
    {
        SetTheme(!IsGalaxyTheme);
    }

    public void SetTheme(bool isGalaxyTheme)
    {
        IsGalaxyTheme = isGalaxyTheme;

        string themeFile = IsGalaxyTheme
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
        catch
        {
            _notifications.ShowError("Error", "Theme could not be loaded.");
        }
    }
}
