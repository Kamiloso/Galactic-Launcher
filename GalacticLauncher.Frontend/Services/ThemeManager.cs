using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Markup.Xaml.Styling;
using Avalonia;

namespace GalacticLauncher.Frontend.Services
{
    internal class ThemeManager
    {
        private bool _isGalaxyTheme = true;
        public event Action<string>? ThemeErrorOccurred;
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
}
