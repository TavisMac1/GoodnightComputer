using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GoodnightComputer.State;

public static class UserExclusionState
{
    private static readonly string _filePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "GoodnightComputer",
        "exclusions.json");

    private static List<string> _exclusions = Load();

    public static IReadOnlyList<string> Exclusions => _exclusions;

    public static void Add(string appName)
    {
        var lower = appName.Trim().ToLower();
        if (!string.IsNullOrWhiteSpace(lower) && !_exclusions.Contains(lower))
        {
            _exclusions.Add(lower);
            Save();
        }
    }

    public static void Remove(string appName)
    {
        var lower = appName.Trim().ToLower();
        if (_exclusions.Remove(lower))
            Save();
    }

    public static bool Contains(string appName) =>
        _exclusions.Contains(appName.Trim().ToLower());

    private static List<string> Load()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            if (File.Exists(_filePath))
                return JsonSerializer.Deserialize<List<string>>(File.ReadAllText(_filePath)) ?? new();
        }
        catch { }
        return new();
    }

    private static void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            File.WriteAllText(_filePath, JsonSerializer.Serialize(_exclusions));
        }
        catch { }
    }
}
