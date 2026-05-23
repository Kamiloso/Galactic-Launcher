using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace GalacticLauncher.Frontend.Services.Executables;

public interface IExecRunner
{
    event Action? OnExecRun;
    void Run(string execPath, string cli);
}

internal class ExecRunner : IExecRunner
{
    public event Action? OnExecRun;

    private Process? _process;

    public void Run(string execPath, string cli)
    {
        if (_process != null)
            throw new InvalidOperationException("An executable has already been run.");

        _process = RunProcess(execPath, cli);

        OnExecRun?.Invoke(); // only if no exception was thrown before
    }

    private static Process RunProcess(string execPath, string cli)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            UnixFileMode mode = File.GetUnixFileMode(execPath);
            if (!mode.HasFlag(UnixFileMode.UserExecute))
            {
                var newMode = mode | UnixFileMode.UserExecute;
                File.SetUnixFileMode(execPath, newMode);
            }
        }

        ProcessStartInfo startInfo = new()
        {
            UseShellExecute = false,
            WorkingDirectory = Path.GetDirectoryName(execPath) ?? "",
            FileName = execPath,
            Arguments = cli,
        };

        return Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Unable to start the process: {execPath}");
    }
}
