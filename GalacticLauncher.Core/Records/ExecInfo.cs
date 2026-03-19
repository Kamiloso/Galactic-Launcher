using GalacticLauncher.Core.Enums;
using System;

namespace GalacticLauncher.Core.Records;

public record ExecInfo(
    string DownloadUrl, // where is it in web?
    string ExecLocation, // where is it in folder?
    string FileHashSHA256, // for integrity & auto-patching
    PlatformType Platform, // Windows / Linux / macOS etc.
    AlertType Alert // how safe is it to run?
    )
{
    public bool RunnableOnThisPlatform() =>
        Platform == Utils.CurrentPlatform;
}
