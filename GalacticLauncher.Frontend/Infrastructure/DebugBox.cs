using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace GalacticLauncher.Frontend.Infrastructure;

public static class DebugBox
{
    public static void Show(string message, string title = "DEBUG ALERT")
    {
        var window = new Window
        {
            Title = title,
            Width = 450,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Topmost = true,
            Content = new Border
            {
                BorderBrush = Brushes.Red,
                BorderThickness = new Thickness(2),
                Padding = new Thickness(20),
                Child = new TextBlock
                {
                    Text = message,
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 16,
                    FontWeight = FontWeight.Bold
                }
            }
        };

        window.Show();
    }
}
