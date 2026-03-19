using System;
using System.Runtime.InteropServices;
using GalacticLauncher.Core.Enums;

namespace GalacticLauncher.Core;

// This class stores universal static methods.
// Put anything you want in here...
public static class Utils
{
    
#if RELEASE
    public static string Address => "https://se3.page:27687";
    public static string CertThumbprint => "NO_RELEASE_CERT_YET";
#elif DEBUG
    public static string Address => "https://localhost:7279";
    public static string CertThumbprint => "36c629df8c377e205faff25b2907d3cca05b0a06534d6c6322a5d04c5275e30e";
#endif

    public static PlatformType CurrentPlatform
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return PlatformType.Windows;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return PlatformType.Linux;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return RuntimeInformation.ProcessArchitecture == Architecture.Arm64
                    ? PlatformType.MacSilicon
                    : PlatformType.MacIntel;
            }

            return PlatformType.Unknown;
        }
    }
}
