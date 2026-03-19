using System;

namespace GalacticLauncher.Core.Enums;

public enum AlertType
{
    Ok = 0,           // No known problems. Considered stable.
    Deprecated = 1,   // Unsupported, shouldn't be used.
    Warning = 2,      // Contains a game-breaking bug.
    Danger = 3,       // Has a major security / data-corrupting issue.
}
