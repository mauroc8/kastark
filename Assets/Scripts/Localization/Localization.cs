using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

// https://www.youtube.com/watch?v=c-dzg4M20wY

public enum Language {
    English, Spanish, Count
};

public static class Localization
{
    static void Init() {
        _englishLanguage = new Dictionary<string, string>();
        _spanishLanguage = new Dictionary<string, string>();

        char lineSeparator = '\n';
        char surround      = '"';
        Regex fieldSeparator = new Regex("\", *\"");
        
        var csvTranslations = Resources.Load<TextAsset>("translations");
        var text = csvTranslations.text;

        string[] lines = text.Split(lineSeparator);

        for (int i = 1; i < lines.Length; i++) {
            string line = lines[i].TrimStart(' ', surround).TrimEnd(surround, '\r');

            if (line.Length == 0 || line[0] == '#') continue; 

            string[] fields = fieldSeparator.Split(line);

            if (fields.Length - 1 != (int) Language.Count) {
                Debug.LogError($"Parse error in translations.csv in line {i}: {line}. Only {fields.Length} fields were found (required {(int) Language.Count + 1})");
            }

            string key = fields[0];

            if (_englishLanguage.ContainsKey(key)) {
                Debug.LogWarning($"Duplicated key \"{key}\" in translations.csv:{i}");
            }

            _englishLanguage.Add(key, ParseField(fields[1]));
            _spanishLanguage.Add(key, ParseField(fields[2]));
        }
    }

    static string ParseField(string field)
    {
        return field.Replace("\\\"", "\"").Replace("\\n", "\n");
    }

    static Dictionary<string, string> _englishLanguage = null;
    static Dictionary<string, string> _spanishLanguage = null;

    static Language _language;

    public static void SetLanguage(Language newLang) {
        _language = newLang;
    }

    public static Language GetLanguage()
    {
        return _language;
    }

    public static string GetLocalizedString(string key)
    {
        string value;

        if (_englishLanguage == null) Init();

        var dictionary = _language == Language.English ? _englishLanguage :
                         _language == Language.Spanish ? _spanishLanguage : null;
        
        if (dictionary == null || !dictionary.TryGetValue(key, out value))
            value = key;

        return value;
    }
}
