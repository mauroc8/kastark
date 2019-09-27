using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Kastark/Game Settings")]

public class GameSettings : ScriptableObject
{
    public Language language = Language.English;

    public void Init() {
        Localization.InitLanguage(language);
    }

    public void Save() {
        PlayerPrefs.SetInt("lang", (int) language);
    }

    public void Load() {
        language = (Language) PlayerPrefs.GetInt("lang");
    }
}
