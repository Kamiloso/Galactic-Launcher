using System.Text.Json;
using System.Text.RegularExpressions;

namespace GalacticLauncher.Core;

public static partial class TextUtils
{
    [GeneratedRegex(@"\{.*?\}")]
    private static partial Regex FormatPlaceholderRegex();

    public static string FormatString(string message, object?[] args)
    {
        int put = 0;
        return FormatPlaceholderRegex()
            .Replace(message, _ =>
            {
                if (put >= args.Length) return "";

                object? arg = args[put++];

                if (arg is null) return "null";

                return JsonSerializer.Serialize(arg, arg.GetType());
            });
    }
}
