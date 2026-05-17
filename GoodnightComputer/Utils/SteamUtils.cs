using GoodnightComputer.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GoodnightComputer.Utils;

public static class SteamUtils
{
    private const string SteamRegistryKey = @"Software\Valve\Steam";

    /// <summary>
    /// Reads the local Steam installation to determine whether a game is running
    /// or a download is in progress. No network calls or API keys required.
    /// </summary>
    public static SteamStatus GetStatus()
    {
        string? steamPath = GetSteamPath();
        if (steamPath == null || !Directory.Exists(steamPath))
            return new SteamStatus { IsSteamInstalled = false };

        int runningAppId = GetRunningAppId();
        var manifests = ReadAllManifests(steamPath);

        string? runningGameName = runningAppId > 0
            ? manifests.GetValueOrDefault(runningAppId)?.Name
            : null;

        var downloading = manifests.Values.FirstOrDefault(m => m.IsDownloading);

        return new SteamStatus
        {
            IsSteamInstalled    = true,
            IsGameRunning       = runningAppId > 0,
            RunningGameName     = runningGameName ?? (runningAppId > 0 ? $"AppID {runningAppId}" : null),
            IsDownloading       = downloading != null,
            DownloadingGameName = downloading?.Name,
            DownloadedBytes     = downloading?.BytesDownloaded ?? 0,
            TotalBytes          = downloading?.BytesToDownload ?? 0,
        };
    }

    private static string? GetSteamPath()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(SteamRegistryKey);
            return key?.GetValue("SteamPath") as string;
        }
        catch { return null; }
    }

    private static int GetRunningAppId()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(SteamRegistryKey);
            var value = key?.GetValue("RunningAppID");
            return value is int id ? id : 0;
        }
        catch { return 0; }
    }

    private static Dictionary<int, ManifestInfo> ReadAllManifests(string steamPath)
    {
        var result = new Dictionary<int, ManifestInfo>();
        foreach (var folder in GetLibraryFolders(steamPath))
        {
            var steamapps = Path.Combine(folder, "steamapps");
            if (!Directory.Exists(steamapps)) continue;
            foreach (var file in Directory.GetFiles(steamapps, "appmanifest_*.acf"))
            {
                var info = ParseManifest(file, steamapps);
                if (info != null)
                    result[info.AppId] = info;
            }
        }
        return result;
    }

    private static List<string> GetLibraryFolders(string steamPath)
    {
        var folders = new List<string> { steamPath };
        try
        {
            var vdfPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(vdfPath)) return folders;
            var content = File.ReadAllText(vdfPath);
            foreach (Match m in Regex.Matches(content, @"""path""\s+""([^""]+)"""))
            {
                var folder = m.Groups[1].Value.Replace(@"\\", @"\");
                if (Directory.Exists(folder) && !folders.Contains(folder, StringComparer.OrdinalIgnoreCase))
                    folders.Add(folder);
            }
        }
        catch { }
        return folders;
    }

    private static ManifestInfo? ParseManifest(string path, string steamappsDir)
    {
        try
        {
            var content = File.ReadAllText(path);
            int appId = ParseIntField(content, "appid");
            if (appId == 0) return null;

            string name = ParseStringField(content, "name") ?? $"AppID {appId}";
            int stateFlags = ParseIntField(content, "StateFlags");
            long bytesDownloaded = ParseLongField(content, "BytesDownloaded");
            long bytesToDownload = ParseLongField(content, "BytesToDownload");

            // StateFlags bit 1024 = actively downloading
            bool isDownloading = (stateFlags & 1024) != 0 && bytesToDownload > 0;

            // When actively downloading, Steam only flushes BytesDownloaded to the
            // manifest on pause/stop. For live progress, measure the downloading
            // folder size — Steam writes chunks there in real time.
            if (isDownloading)
            {
                long liveBytes = GetLiveDownloadedBytes(steamappsDir, appId);
                if (liveBytes > bytesDownloaded)
                    bytesDownloaded = liveBytes;
            }

            return new ManifestInfo
            {
                AppId           = appId,
                Name            = name,
                IsDownloading   = isDownloading,
                BytesDownloaded = bytesDownloaded,
                BytesToDownload = bytesToDownload,
            };
        }
        catch { return null; }
    }

    /// <summary>
    /// Sums the size of all files in steamapps\downloading\{appId}\ to get
    /// the real-time downloaded byte count while a download is active.
    /// </summary>
    private static long GetLiveDownloadedBytes(string steamappsDir, int appId)
    {
        try
        {
            var downloadDir = Path.Combine(steamappsDir, "downloading", appId.ToString());
            if (!Directory.Exists(downloadDir)) return 0;

            return Directory
                .EnumerateFiles(downloadDir, "*", SearchOption.AllDirectories)
                .Sum(f =>
                {
                    try { return new FileInfo(f).Length; }
                    catch { return 0L; }
                });
        }
        catch { return 0; }
    }

    private static string? ParseStringField(string content, string key)
    {
        var match = Regex.Match(content, $@"""{Regex.Escape(key)}""\s+""([^""]*)""", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : null;
    }

    private static int ParseIntField(string content, string key)
        => int.TryParse(ParseStringField(content, key), out int v) ? v : 0;

    private static long ParseLongField(string content, string key)
        => long.TryParse(ParseStringField(content, key), out long v) ? v : 0;

    private class ManifestInfo
    {
        public int    AppId           { get; init; }
        public string Name            { get; init; } = string.Empty;
        public bool   IsDownloading   { get; init; }
        public long   BytesDownloaded { get; init; }
        public long   BytesToDownload { get; init; }
    }
}
