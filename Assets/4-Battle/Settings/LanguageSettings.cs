using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

[CreateAssetMenu(fileName = "LanguageSettings", menuName = "Kastark/Settings/Language")]

public class LanguageSettings : ScriptableObject
{
    public Language language = Language.English;

    public void Init() {
        Localization.SetLanguage(language);
    }

    public void ChangeLanguage(Language newLanguage)
    {
        language = newLanguage;
        Localization.SetLanguage(newLanguage);
        //EventController.TriggerEvent(new LanguageChangeEvent{});
    }

    public void Save() {
        PlayerPrefs.SetInt("GameSettings.language", (int) language);
    }

    public void Load() {
        language = (Language) PlayerPrefs.GetInt("GameSettings.language");
    }
}
