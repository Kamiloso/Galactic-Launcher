using GalacticLauncher.Core.Enums;

namespace GalacticLauncher.Core.Records;

internal record ActionInfo(
    ulong Id, // PK
    ActionType Type, // types: Download, Login etc.
    DateTime Time, // when it was made
    ulong IdUser, // FK
    ulong? IdExec // FK?
    );