﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Kastark/Creature")]
public class Creature : ScriptableObject
{
    [Header("Show")]
    public string creatureName;
    public CreatureKind species;
    public Team team;

    [Header("Stats")]
    [SerializeField] float _initialMaxHealth = 10;
    [SerializeField] float _initialPhysicalResistance = 1;
    [SerializeField] float _initialMagicalResistance = 1;
    [SerializeField] Hability[] _initialHabilities = null;
    [SerializeField] Consumable[] _initialConsumables = null;

    [System.NonSerialized] public float health;
    [System.NonSerialized] public float maxHealth;
    [System.NonSerialized] public float shield;
    [System.NonSerialized] public float physicalResistance;
    [System.NonSerialized] public float magicalResistance;
    [System.NonSerialized] public List<Hability> habilities;
    [System.NonSerialized] public List<Consumable> consumables;

    public void Init()
    {
        health = maxHealth = _initialMaxHealth;
        shield = 0;
        physicalResistance = _initialPhysicalResistance;
        magicalResistance = _initialMagicalResistance;
        habilities = new List<Hability>(_initialHabilities);
        consumables = new List<Consumable>(_initialConsumables);

        foreach (var hability in habilities)
        {
            hability.Init();
        }
        foreach (var consumable in consumables)
        {
            consumable.Init();
        }
    }

    public void TurnStart()
    {
        shield = 0; // shields only last one turn
    }
}
