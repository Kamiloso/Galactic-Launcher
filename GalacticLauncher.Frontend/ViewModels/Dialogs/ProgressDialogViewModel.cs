using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
    private string _progressText = "";
    
    [ObservableProperty]
    private bool _isIndeterminate;

    private readonly Action? _onCancel;
    private bool _isFinished;

    public ProgressDialogViewModel(string title, string message, Action? onCancel)
    {
        Title = title;
        Message = message;
        _onCancel = onCancel;
    }

    [RelayCommand]
    private void Cancel()
    {
        if (_isFinished) return;

        _onCancel?.Invoke();
    }

    public void Finish()
    {
        if (_isFinished) return;
        _isFinished = true;

        Close(0);
    }
}