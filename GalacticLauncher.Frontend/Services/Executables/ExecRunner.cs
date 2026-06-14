using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace GalacticLauncher.Frontend.Services.Executables;

public interface IExecRunner
{
    Process? RunProcess(string execPath, string cli);
}

internal class ExecRunner : IExecRunner
{
    public Process? RunProcess(string execPath, string cli)
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

        return Process.Start(startInfo);
    }
}
