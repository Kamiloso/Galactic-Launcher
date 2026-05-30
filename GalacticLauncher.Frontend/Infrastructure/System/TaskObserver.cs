using System;
using System.Threading;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Tools.Classes;

internal class TaskObserver : IDisposable
{
    public bool IsRunning => _task != null && !_task.IsCompleted;

    private CancellationTokenSource? _cts;
    private Task? _task;

    public Task Start(Func<CancellationToken, Task> run)
    {
        Terminate();

        _cts = new CancellationTokenSource();
        _task = run(_cts.Token);

        return _task;
    }

    public void Terminate()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        _task = null;
    }

    public void Dispose()
    {
        Terminate();
    }
}
