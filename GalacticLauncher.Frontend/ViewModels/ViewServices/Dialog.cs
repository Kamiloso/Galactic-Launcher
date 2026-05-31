using System;
using System.Threading.Tasks;
using GalacticLauncher.Frontend.ViewModels.Dialogs;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

internal interface IDialog
{
    event Action<object?>? OnDialogRequested;
    
    Task<TResult> ShowDialogAsync<TResult>(DialogViewModel<TResult> dialogVm);
    void ShowDialogAndForget<TResult>(DialogViewModel<TResult> dialogVm);
}

internal class Dialog : IDialog
{
    public event Action<object?>? OnDialogRequested;

    public async Task<TResult> ShowDialogAsync<TResult>(DialogViewModel<TResult> dialogVm)
    {
        OnDialogRequested?.Invoke(dialogVm);
        TResult result = await dialogVm.Result;
        OnDialogRequested?.Invoke(null);
        
        return result;
    }
    
    public void ShowDialogAndForget<TResult>(DialogViewModel<TResult> dialogVm)
    {
        _ = ShowDialogAsync(dialogVm);
    }
}