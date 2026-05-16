using System;
using System.IO;
using System.Text.Json;

namespace GoodnightComputer.State;

public static class ScannerSettings
{
    private static readonly string _filePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "GoodnightComputer",
        "scanner-settings.json");

    private static ScannerSettingsData _data = Load();

    /// <summary>
    /// Minimum average CPU % an app must exceed to be included in an auto-generated playlist.
    /// </summary>
    public static double CpuThreshold
    {
        get => _data.CpuThreshold;
        set
        {
            _data.CpuThreshold = value;
            _data.IsUserDefined = true;
            Save();
        }
    }

    /// <summary>
    /// True if the user has explicitly saved a threshold; false if still on the default.
    /// </summary>
    public static bool IsUserDefined => _data.IsUserDefined;

    private static ScannerSettingsData Load()
    {
        try
        {
            if (File.Exists(_filePath))
                return JsonSerializer.Deserialize<ScannerSettingsData>(File.ReadAllText(_filePath)) ?? new();
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

    private class ScannerSettingsData
    {
        public double CpuThreshold { get; set; } = 5.0;
        public bool IsUserDefined { get; set; } = false;
    }
}
