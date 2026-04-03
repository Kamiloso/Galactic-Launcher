namespace GalacticLauncher.Core.DbRecords;

public record GameInfo
{
    public required long Id { get; init; } // PK
    public required string Name { get; init; } // name of the game
    public required string Description { get; init; } // why you should play etc.    
}