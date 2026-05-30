using GalacticLauncher.Core;
using GalacticLauncher.Frontend.Domain.Models;
using System.IO;

namespace GalacticLauncher.Frontend.Services.Executables;

public interface IExecPathSystem
{
    string PrepareGamePath(ExecInfo execInfo, bool ensure);
    string PrepareExecPath(ExecInfo execInfo, bool ensure);
    string PrepareInstancePath(ExecInfo execInfo, bool ensure);
    string? FindExecFilePath(ExecInfo execInfo);
}

internal class ExecPathSystem : IExecPathSystem
{
    public string PrepareGamePath(ExecInfo execInfo, bool ensure)
    {
        return PreparePath(Path.Combine(
            Utils.RootPath,
            execInfo.GameUnique), ensure);
    }

    public string PrepareExecPath(ExecInfo execInfo, bool ensure)
    {
        return PreparePath(Path.Combine(
            Utils.RootPath,
            execInfo.GameUnique,
            execInfo.VersionUnique), ensure);
    }

    public string PrepareInstancePath(ExecInfo execInfo, bool ensure)
    {
        return PreparePath(Path.Combine(
            Utils.RootPath,
            execInfo.GameUnique,
            execInfo.VersionUnique, "Instance"), ensure);
    }

    private static string PreparePath(string directory, bool ensure)
    {
        if (ensure)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        return directory;
    }

    public string? FindExecFilePath(ExecInfo execInfo)
    {
        string instancePath = PrepareInstancePath(execInfo, true);
        string execLocation = execInfo.ExecLocation;

        string execFilePath = Path.Combine(instancePath, execLocation);

        if (!Utils.IsPathInside(execFilePath, instancePath))
            return null;

        if (!File.Exists(execFilePath))
            return null;

        return execFilePath;
    }
}
