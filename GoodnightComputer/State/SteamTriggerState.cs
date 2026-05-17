using System;
using System.IO;
using System.Text.Json;

namespace GoodnightComputer.State;

public static class SteamTriggerState
{
    private static readonly string _filePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "GoodnightComputer",
        "steam-triggers.json");

    private static SteamTriggerData _data = Load();

    /// <summary>Playlist ID to activate when a Steam game is running. Null = no trigger.</summary>
    public static Guid? GameRunningPlaylistId
    {
        get => _data.GameRunningPlaylistId;
        set { _data.GameRunningPlaylistId = value; Save(); }
    }

    /// <summary>Playlist ID to activate when a Steam download is in progress. Null = no trigger.</summary>
    public static Guid? DownloadingPlaylistId
    {
        get => _data.DownloadingPlaylistId;
        set { _data.DownloadingPlaylistId = value; Save(); }
    }

    private static SteamTriggerData Load()
    {
        try
        {
            if (File.Exists(_filePath))
                return JsonSerializer.Deserialize<SteamTriggerData>(File.ReadAllText(_filePath)) ?? new();
        }
        catch { }
        return new();
    }

    private static void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            File.WriteAllText(_filePath, JsonSerializer.Serialize(_data));
        }
        catch { }
    }

    private class SteamTriggerData
    {
        public Guid? GameRunningPlaylistId { get; set; }
        public Guid? DownloadingPlaylistId { get; set; }
    }
}
