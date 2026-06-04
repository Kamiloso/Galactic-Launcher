using System;
using System.Threading.Tasks;
using GalacticLauncher.Frontend.ViewModels.Dialogs;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

internal interface IDialogs
{
    event Action<object?>? OnDialogRequested;
    Task<TResult> ShowDialogAsync<TResult>(DialogViewModel<TResult> dialogVm);
}

internal class Dialogs : IDialogs
{
    public event Action<object?>? OnDialogRequested;

    public async Task<TResult> ShowDialogAsync<TResult>(DialogViewModel<TResult> dialogVm)
    {
        OnDialogRequested?.Invoke(dialogVm);

        TResult result = await dialogVm.GetResultInternal();

        OnDialogRequested?.Invoke(null);
        
        return result;
    }
}