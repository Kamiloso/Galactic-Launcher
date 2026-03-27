using System.Collections;
using System.Runtime.InteropServices;
using GalacticLauncher.Core.Enums;

namespace GalacticLauncher.Core;

public static class Utils
{
    public static bool IsDebug => HasArgumentCLI("--debug");
    public static bool IsRelease => !IsDebug;

    public static string Address => IsRelease
        ? "https://api-galactic.se3.page:27687"
        : "https://localhost:27687";

    public static string CertThumbprint => IsRelease
        ? "990a6a6647d286c7f22badf4a1bcf534b64eb372f8daee4802fccc023cb04467"
        : "8d2df4330cc5662ea74196ab3c1958c51f0ce45ce9143f2bb8e77fc4d6126005";

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

    public static bool HasArgumentCLI(string arg)
    {
        var argList = Environment.GetCommandLineArgs();
        return argList.Any(a => a.Equals(arg, StringComparison.OrdinalIgnoreCase));
    }

    public static bool IsNullRecursive(object? obj)
    {
        if (obj is null) return true;
        if (obj.GetType().IsPrimitive || obj is string || obj.GetType().IsEnum) return false;

        if (obj is IEnumerable enumerable)
            return enumerable.Cast<object>().Any(IsNullRecursive);

        return obj.GetType().GetProperties().Any(prop => IsNullRecursive(prop.GetValue(obj)));
    }
}
