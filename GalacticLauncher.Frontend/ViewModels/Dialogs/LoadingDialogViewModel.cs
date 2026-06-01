using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GalacticLauncher.Frontend.ViewModels.Dialogs;

internal partial class LoadingDialogViewModel : DialogViewModel<bool>
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _message;

    [ObservableProperty]
    private double _progressValue;

    private bool _isFinished;

    public LoadingDialogViewModel(string title, string message)
    {
        Title = title;
        Message = message;
        
        _ = StartFakeProgressAsync();
    }

    private async Task StartFakeProgressAsync()
    {
        while (!_isFinished && ProgressValue < 95)
        {
            await Task.Delay(30); 
            
            if (!_isFinished)
            {
                ProgressValue += (95 - ProgressValue) * 0.05; 
            }
        }
    }

    public async Task Finish()
    {
        _isFinished = true;

        ProgressValue = 100;

        Close(true);
    }
}