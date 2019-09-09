using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField] CharacterStats _stats = null;

    public float health;
    public float maxHealth;
    public float shield;

    void Start() {
        health = _stats.health;
        maxHealth = _stats.maxHealth;
        shield = 0;

        var uiCharacterHealthbar = GetComponentInChildren<UI_CharacterHealthbar>();
        uiCharacterHealthbar.BelongsToCreature(this);
    }

    
}
