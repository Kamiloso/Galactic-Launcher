using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GalacticLauncher.Frontend.ViewModels.Dialogs;

internal partial class LoadingDialogViewModel : DialogViewModel<int>
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _message;

    private bool _mustBeVisible = true;

    public LoadingDialogViewModel(string title, string message,
        int minimumTimeMs = 0)
    {
        Title = title;
        Message = message;

        _ = DelayVisible();

        async Task DelayVisible()
        {
            await Task.Delay(minimumTimeMs); // just to avoid flashing

            _mustBeVisible = false;
        }
    }

    public async Task Finish()
    {
        while (_mustBeVisible)
        {
            await Task.Delay(50);
        }

        Close(0);
    }
}