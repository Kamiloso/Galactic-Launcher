using System;
using System.Threading;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Tools.Classes;

internal class TaskObserver : IDisposable
{
    private CancellationTokenSource? _cts;
    private Task? _task;

    public void Start(Func<CancellationToken, Task> run)
    {
        Terminate();

        _cts = new CancellationTokenSource();
        _task = run(_cts.Token);
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

    public async Task AwaitableTask()
    {
        if (_task != null)
        {
            await _task;
        }
    }

    public void Dispose()
    {
        Terminate();
    }
}
