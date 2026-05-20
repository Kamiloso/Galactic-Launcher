using System.Collections;
using System.Runtime.InteropServices;

namespace GalacticLauncher.Core;

public static class Utils
{
    public static string AppName => "GalacticLauncher";

    public static bool IsProduction => !IsDevelopment;
    public static bool IsDevelopment =>
        HasArgumentCLI("--debug") ||
        HasArgumentCLI("--development") ||
        HasArgumentCLI("--dev");

    public static string Address => IsProduction
        ? "https://api-galactic.se3.page:27687"
        : "https://localhost:27687";

    public static string CertThumbprint => IsProduction
        ? "990a6a6647d286c7f22badf4a1bcf534b64eb372f8daee4802fccc023cb04467"
        : "8d2df4330cc5662ea74196ab3c1958c51f0ce45ce9143f2bb8e77fc4d6126005";

    public static Platform CurrentPlatform
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Platform.Windows;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return Platform.Linux;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return RuntimeInformation.ProcessArchitecture == Architecture.Arm64
                    ? Platform.MacSilicon
                    : Platform.MacIntel;
            }

            throw new PlatformNotSupportedException();
        }
    }

    public static string RootPath
    {
        get
        {
            string basePath = CurrentPlatform switch
            {
                // Windows: %LocalAppData%
                Platform.Windows => Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),

                // Linux: ~/.local/share
                Platform.Linux => Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),

                // macOS: ~/Library/Application Support
                Platform.MacSilicon or Platform.MacIntel => Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Library",
                    "Application Support"),

                _ => throw new PlatformNotSupportedException()
            };

            return Path.Combine(basePath, AppName);
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

        if (obj.GetType().IsPrimitive ||
            obj is string ||
            obj.GetType().IsEnum) return false;

        if (obj is IEnumerable enumerable)
            return enumerable.Cast<object>().Any(IsNullRecursive);

        return obj
            .GetType()
            .GetProperties()
            .Any(prop => IsNullRecursive(prop.GetValue(obj)));
    }

    public static bool IsPathInside(string path, string basePath)
    {
        string fullPath = Normalize(path);
        string fullBasePath = Normalize(basePath);

        var comparison = CurrentPlatform == Platform.Linux
            ? StringComparison.Ordinal
            : StringComparison.OrdinalIgnoreCase;

        return fullPath.StartsWith(fullBasePath, comparison);

        // Hack: We append a trailing separator everywhere.
        static string Normalize(string path)
        {
            path = Path.GetFullPath(path);
            return path.EndsWith(Path.DirectorySeparatorChar)
                ? path
                : path + Path.DirectorySeparatorChar;
        }
    }
}
