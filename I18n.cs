using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Opos;

public static class I18n
{
    private static Dictionary<string, string> _translations = new();
    private static string _currentLanguage = "en";
    private static string _jsonPath;

    public static string CurrentLanguage => _currentLanguage;

    public static void Initialize()
    {
        _jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "translations.json");
        if (!File.Exists(_jsonPath)) return;

        string json = File.ReadAllText(_jsonPath);
        var root = JsonSerializer.Deserialize<JsonElement>(json);

        string defaultLang = root.GetProperty("defaultLanguage").GetString() ?? "en";
        _currentLanguage = defaultLang;

        var languages = root.GetProperty("languages");
        if (languages.TryGetProperty(_currentLanguage, out var langElement))
        {
            LoadLanguage(langElement);
        }
    }

    public static void SetLanguage(string langCode)
    {
        if (!File.Exists(_jsonPath)) return;

        string json = File.ReadAllText(_jsonPath);
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var languages = root.GetProperty("languages");
        if (languages.TryGetProperty(langCode, out var langElement))
        {
            _currentLanguage = langCode;
            LoadLanguage(langElement);
            PersistLanguage(langCode);
        }
    }

    private static void LoadLanguage(JsonElement langElement)
    {
        _translations = new Dictionary<string, string>();
        foreach (var prop in langElement.EnumerateObject())
        {
            _translations[prop.Name] = prop.Value.GetString() ?? "";
        }
    }

    private static void PersistLanguage(string langCode)
    {
        string json = File.ReadAllText(_jsonPath);
        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var options = new JsonSerializerOptions { WriteIndented = true };
        var updatedRoot = new Dictionary<string, object>();
        updatedRoot["defaultLanguage"] = langCode;

        var languagesDict = new Dictionary<string, Dictionary<string, string>>();
        foreach (var lang in root.GetProperty("languages").EnumerateObject())
        {
            var entries = new Dictionary<string, string>();
            foreach (var prop in lang.Value.EnumerateObject())
            {
                entries[prop.Name] = prop.Value.GetString() ?? "";
            }
            languagesDict[lang.Name] = entries;
        }
        updatedRoot["languages"] = languagesDict;

        string updatedJson = JsonSerializer.Serialize(updatedRoot, options);
        File.WriteAllText(_jsonPath, updatedJson);
    }

    public static string T(string key, params object[] args)
    {
        if (_translations.TryGetValue(key, out string? value))
        {
            return args.Length > 0 ? string.Format(value, args) : value;
        }
        return key;
    }

    public static string GetLanguageDisplayName(string langCode)
    {
        return langCode switch
        {
            "en" => "English",
            "es" => "Español",
            _ => langCode
        };
    }
}
