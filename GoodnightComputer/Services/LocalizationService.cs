using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace GoodnightComputer.Services;

/// <summary>
/// Service for managing application localization and language switching
/// </summary>
public class LocalizationService : INotifyPropertyChanged
{
    private string _localizationFolder = string.Empty; // Changed from static readonly to instance field

    private static readonly string _settingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "GoodnightComputer",
        "language.txt");

    private Dictionary<string, string> _translations = new();
    private string _currentLanguage = string.Empty; // Initialize to empty so first set will trigger LoadLanguage

    public event PropertyChangedEventHandler? PropertyChanged;

    public static LocalizationService Instance { get; } = new LocalizationService();

    /// <summary>
    /// Available languages with their display names
    /// </summary>
    public Dictionary<string, string> AvailableLanguages { get; } = new()
    {
        { "en", "English" },
        { "it", "Italiano (Italian)" },
        { "es", "Español (Spanish)" },
        { "fr", "Français (French)" },
        { "de", "Deutsch (German)" },
        { "ar", "العربية (Arabic)" },
        { "pt-BR", "Português (Portuguese - Brazil)" },
        { "ru", "Русский (Russian)" },
        { "hi", "हिन्दी (Hindi)" },
        { "zh", "中文 (Chinese Simplified)" },
        { "zh-TW", "繁體中文 (Chinese Traditional)" },
        { "ja", "日本語 (Japanese)" },
        { "ko", "한국어 (Korean)" }
    };

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage != value && AvailableLanguages.ContainsKey(value))
            {
                _currentLanguage = value;
                LoadLanguage(value);
                SaveLanguagePreference(value);
                OnPropertyChanged();
                OnPropertyChanged("Item[]"); // Notify all indexed properties

                // Force refresh all bindings by notifying with empty string
                OnPropertyChanged(string.Empty);
            }
        }
    }

    /// <summary>
    /// Indexer for accessing translations in XAML: {Binding [KeyName]}
    /// </summary>
    public string this[string key]
    {
        get
        {
            if (_translations.TryGetValue(key, out var value))
                return value;

            // Return key if translation not found (helps identify missing translations)
            Console.WriteLine($"Missing translation for key: {key}");
            return $"[{key}]";
        }
    }

    private LocalizationService()
    {
        // Initialize the localization folder path at construction time, not at type load time
        _localizationFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources",
            "Localization");

        // Load saved language preference or default to English
        string savedLanguage = LoadLanguagePreference();
        CurrentLanguage = AvailableLanguages.ContainsKey(savedLanguage) ? savedLanguage : "en";
    }

    private void LoadLanguage(string languageCode)
    {
        try
        {
            string jsonPath = Path.Combine(_localizationFolder, $"{languageCode}.json");

            if (!File.Exists(jsonPath))
            {
                // Fall back to English if requested language not found
                jsonPath = Path.Combine(_localizationFolder, "en.json");
            }

            if (File.Exists(jsonPath))
            {
                string json = File.ReadAllText(jsonPath);
                _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) 
                    ?? new Dictionary<string, string>();
            }
            else
            {
                // If even English isn't found, use empty dictionary
                _translations = new Dictionary<string, string>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading language {languageCode}: {ex.Message}");
            _translations = new Dictionary<string, string>();
        }
    }

    private string LoadLanguagePreference()
    {
        try
        {
            if (File.Exists(_settingsPath))
                return File.ReadAllText(_settingsPath).Trim();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading language preference: {ex.Message}");
        }
        return "en";
    }

    private void SaveLanguagePreference(string languageCode)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
            File.WriteAllText(_settingsPath, languageCode);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving language preference: {ex.Message}");
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
