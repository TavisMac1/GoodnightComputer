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
    /// Minimum average RAM (MB) an app must exceed to be included in an auto-generated playlist.
    /// </summary>
    public static double RamThresholdMB
    {
        get => _data.RamThresholdMB;
        set
        {
            _data.RamThresholdMB = value;
            _data.IsUserDefined = true;
            Save();
        }
    }

    /// <summary>
    /// Whether to use CPU metric when filtering scan results for playlist creation.
    /// </summary>
    public static bool UseCpuMetric
    {
        get => _data.UseCpuMetric;
        set
        {
            _data.UseCpuMetric = value;
            Save();
        }
    }

    /// <summary>
    /// Whether to use RAM metric when filtering scan results for playlist creation.
    /// </summary>
    public static bool UseRamMetric
    {
        get => _data.UseRamMetric;
        set
        {
            _data.UseRamMetric = value;
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
        public double RamThresholdMB { get; set; } = 500.0;
        public bool UseCpuMetric { get; set; } = true;
        public bool UseRamMetric { get; set; } = false;
        public bool IsUserDefined { get; set; } = false;
    }
}
