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
    static Dictionary<string, string> _englishLanguage = null;
    static Dictionary<string, string> _spanishLanguage = null;

    static void Init() {
        _englishLanguage = new Dictionary<string, string>();
        _spanishLanguage = new Dictionary<string, string>();

        var dictionaries = new Dictionary<string, string>[2]{
            _englishLanguage,
            _spanishLanguage
        };

        var csvParser = new CSVParser("textos-localizados");

        var numberOfRows = csvParser.Content.Count;

        for (var i = 1; i < numberOfRows; i++)
        {
            var row = csvParser.Content[i];
            var key = row[0];

            for (var j = 0; j < dictionaries.Length; j++)
            {
                dictionaries[j].Add(key, row[j + 1]);
            }
        }
    }

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
