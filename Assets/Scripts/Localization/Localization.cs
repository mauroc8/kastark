using System.Collections.Generic;
using Events;
using UnityEngine;

namespace StringLocalization
{
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

            var csvParser = new CSVParser("textos-localizados");
            List<List<string>> content = csvParser.Content;

            for (var i = 1; i < content.Count; i++)
            {
                var row = content[i];

                for (var j = 1; j < row.Count; j++)
                {
                    // Empty keys are used to comment things out.
                    if (row[0] == "") continue;
                    
                    var dictionary =
                        j == 1 ? _englishLanguage :
                        j == 2 ? _spanishLanguage :
                        null;
                    
                    if (dictionary != null)
                    {
                        //Debug.Log($"Adding key {row[0]} as {row[j]} to dictionary {j}.");
                        dictionary.Add(row[0], row[j]);
                    }
                }
            }

            // Force initialization.
            CurrentLanguage = CurrentLanguage;
        }

        static Dictionary<string, string> _currentDictionary;

        static Language _language;

        public static Language CurrentLanguage
        {
            get { return _language; }
            set
            {
                _language = value;
                _currentDictionary =
                    _language == Language.English ? _englishLanguage :
                    _language == Language.Spanish ? _spanishLanguage : null;
                EventController.TriggerEvent(new LanguageChangeEvent{});
            }
        }

        public static string GetLocalizedString(string key)
        {
            string value;

            if (_englishLanguage == null) Init();

            if (_currentDictionary == null)
            {
                Debug.LogWarning($"Current dictionary is not set.");
                return key;
            }
            if (!_currentDictionary.TryGetValue(key, out value))
            {
                Debug.LogWarning($"Failed to localize key {key}.");
                return key;
            }

            return value;
        }
    }
}
