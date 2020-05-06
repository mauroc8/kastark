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

    public StateStream<int> shield = new StateStream<int>(0);

    public CreatureTurn Turn => GetComponent<CreatureTurn>();

    // State

    protected StateStream<CreatureState> stateStream =
        new StateStream<CreatureState>(new CreatureState
        {
            lifePoints =
                ListExtension.Repeat(LifePointState.Idle, 30)
        });

    public Stream<CreatureState> State => stateStream;

    protected EventStream<CreatureEvt> eventStream = new EventStream<CreatureEvt>();
    public Stream<CreatureEvt> Events => eventStream;

    protected override void Awake()
    {
        start.Do(() =>
        {
            var state = stateStream.Value;

            state.lifePoints =
                ListExtension.Repeat(LifePointState.Idle, maxHealth);

            stateStream.Value = state;

            shield.Value = 0;
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
        var state = stateStream.Value;

        state.lifePoints =
            state.lifePoints.MapAtIndex(index, _ => LifePointState.Dead);

        stateStream.Push(state);
    }


    public void HealthPotionWasDrank(int heal)
    {
        for (int i = 0; i < heal; i++)
            SpawnLifePoint();
    }

    private void SpawnLifePoint()
    {
        var state = stateStream.Value;

        var indexOfDead = state.lifePoints.IndexOf(LifePointState.Dead);

        if (indexOfDead == -1)
            return;

        state.lifePoints =
            state.lifePoints.MapAtIndex(indexOfDead, _ => LifePointState.Idle);

        stateStream.Push(state);
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

        if (shield.Value > 0)
            newLifePointState = LifePointState.Shielded;
        else if (UnityEngine.Random.Range(0, 100) < 35)
            newLifePointState = LifePointState.Belt;
        else
            newLifePointState = LifePointState.Dance;

        var state = stateStream.Value;

        state.lifePoints =
            state.lifePoints
                .Map(lp => Functions.ChangeLifePointAnimation(lp, newLifePointState));

        stateStream.Push(state);
    }

    public void PauseLifePoints()
    {
        var state = stateStream.Value;

        state.lifePoints =
            state.lifePoints
                .Map(lp => Functions.ChangeLifePointAnimation(lp, LifePointState.Idle));

        stateStream.Push(state);
    }
}
