using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Domain.Models;
using System;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.ViewModels.Dialogs;

internal partial class ProgressDialogViewModel : DialogViewModel<int>
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _message;

    [ObservableProperty]
    private double _progressValue;

    [ObservableProperty]
    private string _progressText;
    
    [ObservableProperty]
    private bool _isIndeterminate;

    public event Action? OnCancel;

    public ProgressDialogViewModel(string title, string message,
        Progress<DownloadProgressData> progress)
    {
        Title = title;
        Message = message;

        IsIndeterminate = false;
        ProgressValue = 0.0;
        ProgressText = "Waiting for download...";

        progress.ProgressChanged += ProgressHandler;
    }

    private void ProgressHandler(object? sender, DownloadProgressData e)
    {
        ProgressValue = e.Percentage;

        if (e.DownloadedBytes < e.TotalBytes)
        {
            double currentMb = e.DownloadedBytes / 1048576.0;
            if (e.TotalBytes.HasValue)
            {
                IsIndeterminate = false;
                ProgressValue = e.Percentage;

                double totalMb = e.TotalBytes.Value / 1048576.0;
                ProgressText = $"{currentMb:F2} MB / {totalMb:F2} MB";
            }
            else
            {
                IsIndeterminate = true;
                ProgressText = $"{currentMb:F2} MB Downloaded";
            }
        }
        else
        {
            IsIndeterminate = true;
            ProgressValue = 1.0;
            ProgressText = "Installing...";
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        OnCancel?.Invoke();
    }

    public async Task Finish()
    {
        Close(0);
    }
}