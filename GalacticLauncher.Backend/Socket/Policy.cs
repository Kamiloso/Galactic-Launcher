namespace GalacticLauncher.Backend.Socket;

internal record Policy(TimeSpan Period, int Limit)
{
    public const string FREE = nameof(FREE);
    public const string LOW_COST = nameof(LOW_COST);
    public const string MEDIUM_COST = nameof(MEDIUM_COST);
    public const string HIGH_COST = nameof(HIGH_COST);
}
