using System;
using System.Collections.Generic;
using UnityEngine;
using ListExtensions;


// State ------------------

public struct CreatureState
{
    public List<LifePointState> lifePoints;
}



// Events ------------------

public interface CreatureEvt
{
}

namespace CreatureEvts
{
    public struct ReceivedDamage : CreatureEvt { }

    public struct ReceivedHeal : CreatureEvt { }
}



// Creature -------------------------------------------------------


public class Creature : StreamBehaviour
{
    [Header("Params")]

    public string creatureName;
    public CreatureKind species;
    public TeamId team;
    public int maxHealth = 10;

    public Transform head;
    public Transform feet;

    public float Height => Vector3.Distance(head.position, feet.position);

    [NonSerialized] public int health = 10;
    [NonSerialized] public int shield = 0;

    public bool IsAlive => health > 0;

    public CreatureTurn Turn => GetComponent<CreatureTurn>();

    // State

    private CreatureState _state;

    protected StreamSource<CreatureState> stateStream = new StreamSource<CreatureState>();
    public Stream<CreatureState> State => stateStream;

    protected StreamSource<CreatureEvt> eventStream = new StreamSource<CreatureEvt>();
    public Stream<CreatureEvt> Events => eventStream;

    protected override void Awake()
    {
        health = maxHealth;

        start.Do(() =>
        {
            _state =
                new CreatureState
                {
                    lifePoints =
                            ListExtension.Repeat(
                                LifePointState.Idle,
                                maxHealth
                            )
                };

            stateStream.Push(_state);
        });

        stateStream
            .Map(state =>
                state.lifePoints
                    .Filter(lifePoint => lifePoint != LifePointState.Dead)
                    .Count)
            .WithLastValue(maxHealth)
            .Get((lastLifePoints, lifePoints) =>
            {
                if (lifePoints < lastLifePoints)
                {
                    eventStream.Push(new CreatureEvts.ReceivedDamage { });
                }
                else if (lifePoints > lastLifePoints)
                {
                    eventStream.Push(new CreatureEvts.ReceivedHeal { });
                }
            });
    }


    // Creature Events ------------------

    public void LifePointWasHit(int index)
    {
        _state.lifePoints =
            _state.lifePoints.MapAtIndex(index, _ => LifePointState.Dead);

        stateStream.Push(_state);
    }


    public void HealthPotionWasDrank(int heal)
    {
        for (int i = 0; i < heal; i++)
            SpawnLifePoint();
    }

    private void SpawnLifePoint()
    {
        var indexOfDead = _state.lifePoints.IndexOf(LifePointState.Dead);

        if (indexOfDead == -1)
            return;

        _state.lifePoints =
            _state.lifePoints.MapAtIndex(indexOfDead, _ => LifePointState.Idle);

        stateStream.Push(_state);
    }

    private Battle __battle = null;
    private Battle _battle => __battle ?? (__battle = GetComponentInParent<Battle>());

    public void HabilityWasSelected(HabilityId habilityId)
    {
        if (habilityId == HabilityId.Attack || habilityId == HabilityId.Magic)
        {
            _battle.CreatureBeganAttack(team);
        }
    }

    public void TurnIsAboutToEnd()
    {
        _battle.CreatureCeasedAttack(team);
    }

    public void AnimateLifePoints()
    {
        LifePointState newLifePointState;

        if (UnityEngine.Random.Range(0, 100) < 35)
            newLifePointState = LifePointState.Belt;
        else
            newLifePointState = LifePointState.Dance;

        _state.lifePoints =
            _state.lifePoints
                .Map(lp => Functions.ChangeLifePointAnimation(lp, newLifePointState));

        stateStream.Push(_state);
    }

    public void PauseLifePoints()
    {
        _state.lifePoints =
            _state.lifePoints
                .Map(lp => Functions.ChangeLifePointAnimation(lp, LifePointState.Idle));

        stateStream.Push(_state);
    }
}
