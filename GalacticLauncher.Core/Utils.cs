using System.Runtime.InteropServices;
using GalacticLauncher.Core.Enums;

namespace GalacticLauncher.Core;

public static class Utils
{
    // backend connection definitions
#if RELEASE
    public static string Address => "https://se3.page:27687";
    public static string CertThumbprint => "NO_RELEASE_CERT_YET";
#elif DEBUG
    public static string Address => "https://localhost:7279";
    public static string CertThumbprint => "36c629df8c377e205faff25b2907d3cca05b0a06534d6c6322a5d04c5275e30e";
#else
#error RELEASE or DEBUG mode must be chosen.
#endif

    // limiting policy strings
    public const string FREE = nameof(FREE);
    public const string LOW_COST = nameof(LOW_COST);
    public const string MEDIUM_COST = nameof(MEDIUM_COST);
    public const string HIGH_COST = nameof(HIGH_COST);

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
