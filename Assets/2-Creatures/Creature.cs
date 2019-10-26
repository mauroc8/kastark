using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [Header("Information and stats")]
    public string creatureName;
    public CreatureKind species;
    public TeamId team;
    public float maxHealth = 10;

    public Transform head;
    public Transform feet;

    [NonSerialized] public float health;
    [NonSerialized] public float shield;

    public bool IsAlive => health > 0;

    [Header("Behaviour related")]
    [SerializeField] HabilitySelection _habilitySelection;

    void Awake()
    {
        health = maxHealth;
    }

    public async Task TurnAsync(CancellationToken token)
    {
        Debug.Log($"{creatureName}'s turn");
        
        var selectedHability = await _habilitySelection.SelectHabilityAsync(token);
        await selectedHability.CastAsync(this, token);
    }
}
