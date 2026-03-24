using GalacticLauncher.Core.Enums;
using System;

namespace GalacticLauncher.Core.DbRecords;

public record GameInfo
{
    public ulong Id { get; init; } // PK
    public string Name { get; init; } = string.Empty; // name of the game
    public string Description { get; init; } = string.Empty; // why you should play etc.
}