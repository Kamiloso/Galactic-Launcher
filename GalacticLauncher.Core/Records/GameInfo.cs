using GalacticLauncher.Core.Enums;
using System;

namespace GalacticLauncher.Core.Records;

public record GameInfo(
    ulong Id, // PK
    string Name, // name of the game
    string Description // why you should play etc.
    );