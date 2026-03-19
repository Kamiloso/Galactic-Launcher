using System;

namespace GalacticLauncher.Core.Enums;

[Flags]
public enum GameTag : long
{
    None = 0,

    // --- Main Genres ---
    Action = 1L << 0,
    Adventure = 1L << 1,
    RPG = 1L << 2,
    Strategy = 1L << 3,
    Simulation = 1L << 4,
    Sports = 1L << 5,
    Racing = 1L << 6,
    Puzzle = 1L << 7,
    Shooter = 1L << 8,
    Platformer = 1L << 9,
    Fighting = 1L << 10,
    Casual = 1L << 11,
    Arcade = 1L << 12,

    // --- Subgenres & Mechanics ---
    Survival = 1L << 13,
    Horror = 1L << 14,
    Sandbox = 1L << 15,
    OpenWorld = 1L << 16,
    Stealth = 1L << 17,
    StoryRich = 1L << 18,

    // --- Game Modes ---
    Singleplayer = 1L << 19,
    Multiplayer = 1L << 20,
    CoOp = 1L << 21,
    MMO = 1L << 22,

    // --- Theme & Origin ---
    SciFi = 1L << 23,
    Fantasy = 1L << 24,
    Indie = 1L << 25
}
