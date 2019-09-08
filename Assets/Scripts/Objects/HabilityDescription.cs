using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HabilityDescription", menuName = "Hability Description")]

public class HabilityDescription : ScriptableObject
{
    public HabilityId habilityId;
    public string habilityName;
    [TextArea(0, 2)]
    public string description;
}
