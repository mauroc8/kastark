using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class AttackController : Ability
{
    public override bool IsAgressive =>
        true;

    interface CutStatus { }

    struct Preparing : CutStatus { }

    struct Ready : CutStatus { }

    struct Cutting : CutStatus
    {
        public AttackTrail attackTrail;
    }

    struct AfterCutting : CutStatus { }

    public int amountOfCuts = 3;

    void Awake()
    {
        var cuts =
            new StateStream<int>(0);

        var cutStatus =
            new StateStream<CutStatus>(new Preparing());

        var creature =
            GetComponentInParent<Creature>();

        var battle =
            GetComponentInParent<Battle>();

        var isCasting =
            battle
                .turn
                .Map(optionalTurn =>
                    optionalTurn.CaseOf(
                        turn =>
                            battle.ActingCreature(turn) == creature
                                && turn.action == TurnAction.CastAbility
                                && turn.selectedAbility == this,
                        () => false
                    )
                )
                .Lazy();

        var castStart =
            isCasting
                .Filter(a => a);

        var castUpdate =
            isCasting
                .AndThen(value =>
                    value ? update : Stream.None<Void>()
                );

        // THIS IS HELL
        castUpdate
            .Get(_ => { });

        // Init state

        castStart
            .Get(_ =>
            {
                cuts.Value =
                    amountOfCuts;

                cutStatus.Value =
                    new Preparing();
            });

        // finish preparing

        var animationEvents =
            Query
                .From(creature)
                .Get<AiraAbilityAnimationEvents>();

        animationEvents
            .swordReady
            .Get(_ =>
            {
                cutStatus.Value =
                    new Ready();
            });

        var trail =
            Query
                .From(this, "trail")
                .Get();

        cutStatus
            .Map(value =>
                Functions.IsTypeOf<CutStatus, Ready>(value)
                    || Functions.IsTypeOf<CutStatus, Preparing>(value))
            .AndThen(value =>
                value && cuts.Value > 0 ? castUpdate : Stream.None<Void>())
            .Filter(_ => Input.GetMouseButtonDown(0))
            .Get(_ =>
            {
                var attackTrail =
                    Instantiate(trail)
                        .GetComponent<AttackTrail>();

                attackTrail
                    .Open(Input.mousePosition);

                cutStatus.Value =
                    new Cutting { attackTrail = attackTrail };
            });

        var cuttingUpdate =
            cutStatus
                .Map(Functions.IsTypeOf<CutStatus, Cutting>)
                .AndThen(value =>
                    value
                        ? castUpdate
                            .Always((Cutting)cutStatus.Value)
                        : Stream.None<Cutting>()
                );

        cuttingUpdate
            .Filter(_ => Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
            .Get(cutting =>
            {
                cutting.attackTrail.Move(Input.mousePosition);
            });

        cuttingUpdate
            .Filter(cutting => cutting.attackTrail.IsOutOfBounds || !Input.GetMouseButton(0))
            .Get(cutting =>
            {
                cutting.attackTrail.Close();

                Debug.Log($"Closing cut ({cutting.attackTrail.PerformedCut})");

                if (cutting.attackTrail.PerformedCut)
                {
                    cuts.Value--;
                }

                cutStatus.Value =
                    new AfterCutting();
            });

        animationEvents
            .swordCast
            .Get(_ =>
            {
                cutStatus.Value =
                    new Preparing();
            });


        var castEnd =
            cuts
                .Filter(value => value <= 0)
                .AndThen(Functions.WaitForSeconds<int>(update, 0.8f));

        castEnd
            .Get(_ =>
            {
                battle
                    .CreatureEndsTurn();
            });

        // FX and animations

        var swooshSound =
            GetComponent<AudioSource>();

        cuttingUpdate
            .Filter(cutting => cutting.attackTrail.PerformedCut)
            .Lazy()
            .Get(_ =>
            {
                Functions.PlaySwooshSound(swooshSound);
            });

        var animator =
            Query
                .From(creature, "mesh")
                .Get<Animator>();

        cutStatus
            .Filter(_ => cuts.Value > 0)
            .Get(status =>
            {
                switch (status)
                {
                    case Preparing _:
                        animator.SetTrigger("attack_sword");

                        animator.SetFloat("attack_sword_speed", 1.0f);

                        break;

                    case Ready _:
                        animator.SetFloat("attack_sword_speed", 0.0f);

                        break;

                    case Cutting _:
                        animator.SetFloat("attack_sword_speed", 1.0f);
                        break;
                }
            });

        // Show cuts bar

        var cutBar =
            Query
                .From(this, "cut-bar")
                .Get();

        cutBar.SetActive(false);

        castStart
            .Get(_ =>
            {
                cutBar.SetActive(true);
            });

        castEnd
            .Get(_ =>
            {
                cutBar.SetActive(false);
            });

        // Show remaining cuts

        var abilityCuts =
            GetComponentsInChildren<AbilityCut>(true);

        cuts
            .Get(value =>
            {
                for (int i = 0; i < amountOfCuts; i++)
                {
                    if (i < value)
                        abilityCuts[i].Filled();
                    else
                        abilityCuts[i].Empty();
                }
            });

        // Show amount of cuts

        for (int i = 0; i < abilityCuts.Length; i++)
        {
            abilityCuts[i].gameObject.SetActive(i < amountOfCuts);
        }

    }
}
