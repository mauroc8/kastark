using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character Attributes")]

public class CharacterAttributes : ScriptableObject
{
    public float health;
    public float maxHealth;
}
