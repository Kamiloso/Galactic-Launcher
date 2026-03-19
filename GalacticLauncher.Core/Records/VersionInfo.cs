using GalacticLauncher.Core.Enums;
using System;

namespace GalacticLauncher.Core.Records;

public record VersionInfo(
    string VersionText, // example: "2.3b"
    string Description, // changelog for example
    DateTime ReleaseDate, // release date
    VersionType VersionType, // Alpha, Beta, Release etc.
    ExecInfo[] ExecInfos, // information about available executables
    string[] ImgUrls // version images
    )
{
    public bool IsRelease() =>
        VersionType == VersionType.Release;

    public ExecInfo? ExecInfoForThisPlatform()
    {
        var supported = ExecInfos
            .Where(e => e.RunnableOnThisPlatform());

        return supported.FirstOrDefault();
    }

    public bool IsAvailableOnThisPlatform() =>
        ExecInfoForThisPlatform() is not null;

    public bool IsAvailable() =>
        ExecInfos.Length > 0;
}
