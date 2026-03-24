using GalacticLauncher.Core.Enums;
using System;

namespace GalacticLauncher.Core.DbRecords;

public record ExecInfo(
    ulong Id, // PK
    string DownloadUrl, // where is it in web?
    string ExecLocation, // where is it in folder?
    string FileHashSHA256, // for integrity & auto-patching
    PlatformType Platform, // Windows / Linux / macOS etc.
    AlertType Alert, // how safe is it to run?
    ulong IdVersion // FK
    );
