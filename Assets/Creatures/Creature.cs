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


public class Creature : StreamBehaviour<CreatureState, CreatureEvt>
{
    [Header("Params")]

    public string creatureName;
    public CreatureKind species;
    public TeamId team;
    public int maxHealth = 10;

    public Transform head;
    public Transform feet;

    public float Height => Vector3.Distance(head.position, feet.position);

    [NonSerialized] public int health;
    [NonSerialized] public float shield;

    public bool IsAlive => health > 0;

    public CreatureTurn Turn => GetComponent<CreatureTurn>();



    protected override void Awake()
    {
        health = maxHealth;

        // Initial state -----------------

        stateStream.Push(new CreatureState
        {
            lifePoints =
                    ListExtension.Repeat(
                        LifePointState.Idle,
                        maxHealth
                    )
        });


        // Battle Events -----------------------------------------


        var battle = GetContext<Battle>();


        battle.EventStream<BattleEvts.CreatureBeganAttack>().Get(evt =>
        {
            if (evt.team != team)
            {
                // Enemy Began Attack.

                ExposeLifePoints();
            }
        });


        battle.EventStream<BattleEvts.CreatureCeasedAttack>().Get(evt =>
        {
            if (evt.team != team)
            {
                // Enemy Is About To End Attack.

                HideLifePoints();
            }
        });
    }



    // Creature Events ------------------------------


    public void LifePointWasHit(int index)
    {
        stateStream.Update(state =>
        {
            state.lifePoints =
                state.lifePoints.MapAtIndex(index, _ => LifePointState.Dead);


            return state;
        });

        eventStream.Push(new CreatureEvts.ReceivedDamage { });
    }


    public void HealthPotionWasDrank(int heal)
    {
        for (int i = 0; i < heal; i++)
            SpawnLifePoint();
    }

    private void SpawnLifePoint()
    {
        stateStream.Update(state =>
        {
            var indexOfFirstDead = state.lifePoints.IndexOf(LifePointState.Dead);

            if (indexOfFirstDead == -1)
                return state;

            state.lifePoints =
                state.lifePoints
                    .MapAtIndex(indexOfFirstDead, _ => LifePointState.Idle);

            return state;
        });

        eventStream.Push(new CreatureEvts.ReceivedHeal { });
    }


    public void HabilityWasSelected(HabilityId habilityId)
    {
        if (habilityId == HabilityId.Attack || habilityId == HabilityId.Magic)
        {
            GetContext<Battle>().CreatureBeganAttack(team);
        }
    }


    public void TurnIsAboutToEnd()
    {
        GetContext<Battle>().CreatureCeasedAttack(team);
    }


    private void ExposeLifePoints()
    {
        stateStream.Update(state =>
        {
            LifePointState newLifePointState;

            if (UnityEngine.Random.Range(0, 100) < 50)
                newLifePointState = LifePointState.Belt;
            else
                newLifePointState = LifePointState.Dance;

            state.lifePoints =
                state.lifePoints
                    .Map(lp => _.ChangeLifePointAnimation(lp, newLifePointState));

            return state;
        });
    }

    private void HideLifePoints()
    {
        stateStream.Update(state =>
        {
            state.lifePoints =
                state.lifePoints
                    .Map(lp => _.ChangeLifePointAnimation(lp, LifePointState.Idle));

            return state;
        });
    }
}
