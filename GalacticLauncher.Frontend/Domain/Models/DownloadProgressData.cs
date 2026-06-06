namespace GalacticLauncher.Frontend.Domain.Models;

public readonly record struct DownloadProgressData(
    double Percentage,
    long DownloadedBytes,
    long? TotalBytes);