using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public float shield;

    void Start() {
        var uiCharacterHealthbar = GetComponentInChildren<UI_CharacterHealthbar>();
        uiCharacterHealthbar.BelongsToCreature(this);
    }

    
}
