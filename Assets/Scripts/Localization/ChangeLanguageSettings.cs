using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLanguageSettings : MonoBehaviour
{
    [SerializeField] GameSettings _gameSettings = null;

    public void ChangeLanguageToSpanish()
    {
        _gameSettings.ChangeLanguage(Language.Spanish);
        _gameSettings.Save();
    }

    public void ChangeLanguageToEnglish()
    {
        _gameSettings.ChangeLanguage(Language.English);
        _gameSettings.Save();
    }
}
