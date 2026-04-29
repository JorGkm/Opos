using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Opos;

public static class I18n
{
    private static Dictionary<string, string> _translations = new();
    private static string _currentLanguage = "en";

    public static string CurrentLanguage => _currentLanguage;

    public static void Initialize()
    {
        string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "translations.json");
        if (!System.IO.File.Exists(jsonPath)) return;

        string json = System.IO.File.ReadAllText(jsonPath);
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
        string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "translations.json");
        if (!System.IO.File.Exists(jsonPath)) return;

        string json = System.IO.File.ReadAllText(jsonPath);
        var root = JsonSerializer.Deserialize<JsonElement>(json);

        var languages = root.GetProperty("languages");
        if (languages.TryGetProperty(langCode, out var langElement))
        {
            _currentLanguage = langCode;
            LoadLanguage(langElement);
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
