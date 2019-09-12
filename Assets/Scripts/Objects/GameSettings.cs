using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Custom/Game Settings")]

public class GameSettings : ScriptableObject
{
    public void Save() {
        PlayerPrefs.SetInt("lang", (int) language);
    }

    public void Load() {
        language = (Language) PlayerPrefs.GetInt("lang");
    }

    public Language language = Language.English;
}
