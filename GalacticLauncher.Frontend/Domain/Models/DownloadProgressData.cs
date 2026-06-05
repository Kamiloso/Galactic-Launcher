namespace GalacticLauncher.Frontend.Domain.Models;

public record struct DownloadProgressData(double Percentage, long DownloadedBytes, long? TotalBytes);