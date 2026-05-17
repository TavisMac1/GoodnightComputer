namespace GoodnightComputer.Models;

public class SteamStatus
{
    public bool IsSteamInstalled { get; init; }
    public bool IsGameRunning { get; init; }
    public string? RunningGameName { get; init; }
    public bool IsDownloading { get; init; }
    public string? DownloadingGameName { get; init; }
    public long DownloadedBytes { get; init; }
    public long TotalBytes { get; init; }

    public string DownloadProgressText =>
        TotalBytes > 0
            ? $"{FormatBytes(DownloadedBytes)} / {FormatBytes(TotalBytes)} ({(double)DownloadedBytes / TotalBytes:P0})"
            : "Calculating…";

    private static string FormatBytes(long bytes)
    {
        if (bytes >= 1_073_741_824) return $"{bytes / 1_073_741_824.0:F1} GB";
        if (bytes >= 1_048_576)     return $"{bytes / 1_048_576.0:F1} MB";
        return $"{bytes / 1024.0:F1} KB";
    }
}
