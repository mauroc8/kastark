using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Creature : MonoBehaviour
{
    [Header("Information and stats")]
    public string creatureName;
    public CreatureKind species;
    public TeamId team;
    public float maxHealth = 10;

    public Transform head;
    public Transform feet;

    public float Height => Vector3.Distance(head.position, feet.position);

    [NonSerialized] public float health;
    [NonSerialized] public float shield;

    public bool IsAlive => health > 0;

    [Header("Behaviour related")]
    [SerializeField] HabilitySelection _habilitySelection;

    [Header("Events")]
    [SerializeField] UnityEvent _turnStartEvent;
    [SerializeField] UnityEvent _habilitySelectedEvent;
    [SerializeField] UnityEvent _turnEndEvent;

    void Awake()
    {
        health = maxHealth;
    }

    public async Task TurnAsync(CancellationToken token)
    {
        Debug.Log($"{creatureName}'s turn");
        _turnStartEvent.Invoke();

        var selectedHability = await _habilitySelection.SelectHabilityAsync(token);
        _habilitySelectedEvent.Invoke();

        await selectedHability.CastAsync(token);
        _turnEndEvent.Invoke();
    }

    public void OnLifePointHit(GameObject lifePoint)
    {
        health--;
    }

}
