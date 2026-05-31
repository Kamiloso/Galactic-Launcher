using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GalacticLauncher.Frontend.ViewModels.Dialogs;

public abstract class DialogViewModel<TResult> : ObservableObject
{
    private readonly TaskCompletionSource<TResult> _tcs = new();
    
    public Task<TResult> Result => _tcs.Task;

    protected void Close(TResult result)
    {
        _tcs.TrySetResult(result);
    }
}