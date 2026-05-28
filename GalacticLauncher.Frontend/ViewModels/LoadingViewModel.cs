using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GalacticLauncher.Frontend.ViewModels;

internal partial class LoadingViewModel : ObservableObject
{
    [ObservableProperty]
    private string _loadingText = ".";

    private int _step = 0;
    private readonly DispatcherTimer _timer;

    public LoadingViewModel()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _timer.Tick += (s, e) => UpdateDots();
        _timer.Start();
    }

    private void UpdateDots()
    {
        _step = (_step % 3) + 1;
        LoadingText = new string('.', _step);
    }
}
