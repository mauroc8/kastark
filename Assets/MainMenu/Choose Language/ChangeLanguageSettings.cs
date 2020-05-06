using System.Collections;
using System.Collections.Generic;
using GlobalEvents;
using UnityEngine;

public class ChangeLanguageSettings : MonoBehaviour
{
    void Start()
    {
        var languageIndex = PlayerPrefs.GetInt("languageIndex", 0);
        Localization.currentLanguage = (LanguageId)languageIndex;
    }

    public void ChangeLanguage(LanguageId languageId)
    {
        Localization.currentLanguage = languageId;
        PlayerPrefs.SetInt("languageIndex", (int)languageId);
        EventController.TriggerEvent(new LanguageChangeEvent { });
    }

    public void ChangeLanguageToSpanish()
    {
        ChangeLanguage(LanguageId.Spanish);
    }

    public void ChangeLanguageToEnglish()
    {
        ChangeLanguage(LanguageId.English);
    }
}
