using System;

namespace GalacticLauncher.Core.Enums;

public enum AlertType
{
    Ok = 0, // No known problems. Considered stable.
    Deprecated = 1, // Unsupported, shouldn't be used.
    Unsafe = 2, // Contains a game-breaking bug.
    Dangerous = 3, // Has a major security / data-corrupting issue.
}
