using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Custom/Game Settings")]

public class GameSettings : ScriptableObject
{
    public Language language = Language.English;
}
