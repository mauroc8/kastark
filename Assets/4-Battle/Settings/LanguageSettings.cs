using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StringLocalization;

[CreateAssetMenu(fileName = "LanguageSettings", menuName = "Kastark/Settings/Language")]

public class LanguageSettings : ScriptableObject
{
    public Language language = Language.English;

    public void Init() {
        Localization.CurrentLanguage = language;
    }

    public void ChangeLanguage(Language newLanguage)
    {
        language = newLanguage;
        Localization.CurrentLanguage = newLanguage;
    }

    public void Save() {
        PlayerPrefs.SetInt("GameSettings.language", (int) language);
    }

    public void Load() {
        language = (Language) PlayerPrefs.GetInt("GameSettings.language");
    }
}
