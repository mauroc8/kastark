using System;
using System.Collections.Generic;
using UnityEngine;
using ListExtensions;
using System.Linq;


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

    // State

    protected StateStream<CreatureState> stateStream =
        new StateStream<CreatureState>(new CreatureState
        {
            lifePoints =
                new List<LifePointState> { }
        });

    public Stream<CreatureState> State => stateStream;

    protected override void Awake()
    {
        stateStream.Value =
            new CreatureState
            {
                lifePoints =
                    ListExtension.Repeat(LifePointState.Idle, maxHealth)
            };

        start.Get(_ =>
        {
            stateStream.Value =
                stateStream.Value;
        });

        var battle =
            GetComponentInParent<Battle>();

        battle
            .turn
            .FilterMap(a => a)
            .Map(turn =>
                turn.action == TurnAction.CastAbility
                    && turn.selectedAbility.IsAgressive
                    && battle.EnemyOf(turn.team) == this.team
            )
            .Lazy()
            .Get(isBeingAttacked =>
            {
                if (isBeingAttacked)
                    AnimateLifePoints();
                else
                    PauseLifePoints();
            });


        // Animate circle

        var isMyTurn =
            battle
                .turn
                .FilterMap(a => a)
                .Map(battle.ToCreature)
                .Map(creature => creature == this)
                .Lazy();


        isMyTurn
            .Get(isActive =>
            {
            });


        var circleRenderer =
            Query
                .From(this, "circle")
                .Get<Renderer>();

        var circleInitialEmission =
            circleRenderer.material.GetColor("_EmissionColor");

        isMyTurn
            .InitializeWith(false)
            .Map(value => value ? 2.0f : 1.0f)
            .AndThen(Functions.LerpStreamOverTime(update, 0.4f))
            .Get(t =>
            {
                circleRenderer.material
                    .SetColor("_EmissionColor", circleInitialEmission * t);
            });

        var circleTransform =
            circleRenderer.transform;



        // Aira flies

        var animator =
            Query
                .From(this, "mesh")
                .Get<Animator>();

        if (team == TeamId.Player)
        {
            animator.SetFloat("vertical_height", 1.0f);
        }

        // Othok is in battle

        if (team == TeamId.Enemy)
        {
            animator.SetBool("is_in_battle", true);

            // Othok gets hit

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
                        if (team == TeamId.Enemy)
                        {
                            animator.SetTrigger("gets_hit");
                        }
                        //eventStream.Push(new CreatureEvts.ReceivedDamage { });
                    }
                    else if (lifePoints > lastLifePoints)
                    {
                        //eventStream.Push(new CreatureEvts.ReceivedHeal { });
                    }
                });
        }
    }


    // Creature Events ------------------

    public void LifePointWasHit(int index)
    {
        var state = stateStream.Value;

        state.lifePoints =
            state.lifePoints.MapAtIndex(index, _ => LifePointState.Dead);

        stateStream.Value =
            state;
    }


    public void HealthPotionWasDrank(int heal)
    {
        for (int i = 0; i < heal; i++)
            SpawnLifePoint();
    }

    private bool SpawnLifePoint()
    {
        var state = stateStream.Value;

        var indexOfDead = state.lifePoints.IndexOf(LifePointState.Dead);

        if (indexOfDead == -1)
            return false;

        state.lifePoints =
            state.lifePoints.MapAtIndex(indexOfDead, _ => LifePointState.Idle);

        stateStream.Value =
            state;

        return true;
    }

    private Battle __battle = null;
    private Battle _battle => __battle ?? (__battle = GetComponentInParent<Battle>());


    void AnimateLifePoints()
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

        stateStream.Value =
            state;
    }

    void PauseLifePoints()
    {
        var state = stateStream.Value;

        state.lifePoints =
            state.lifePoints
                .Map(lp => Functions.ChangeLifePointAnimation(lp, LifePointState.Idle));

        stateStream.Value =
            state;
    }
}
