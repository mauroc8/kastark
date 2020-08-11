using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vector3Extensions;

public class MagicController : Ability
{
    public override bool IsAgressive =>
        true;

    enum ShootState
    {
        Preparing,
        Ready,
        AboutToShoot,
        Shooting
    };

    public int amountOfCuts = 3;

    void Awake()
    {
        var cuts =
            new StateStream<int>(0);

        var shootState =
            new StateStream<ShootState>(ShootState.Preparing);

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

        // Cuts

        castStart
            .Get(_ =>
            {
                cuts.Value =
                    amountOfCuts;

                shootState.Value =
                    ShootState.Preparing;

            });

        //

        var camera =
            Camera.main;

        var focus =
            Query
                .From(this, "focus")
                .Get<Transform>();

        var magicEnergy =
            Query
                .From(this, "magic-energy")
                .Get<Transform>();

        var enemyTeam =
            TeamId.Enemy;

        isCasting
            .InitializeWith(false)
            .Get(focus.gameObject.SetActive);

        // Raycast

        var hitId =
            0;

        castUpdate
            .Get(_ =>
            {
                var position =
                    magicEnergy.position;

                var screenPosition =
                    camera.WorldToScreenPoint(position);

                var target =
                    RaycastHelper.SphereCastAtScreenPoint(screenPosition, LayerMask.HabilityRaycast, 1.2f);

                if (target != null && target.CompareTag(enemyTeam.ToString()))
                {
                    var hittable = target.GetComponent<IHittable>();

                    if (hittable != null)
                        hittable.Hit(this, hitId++);
                }
            });


        // Move with mouse

        var plane =
            new Plane(
                focus.right,
                focus.position
            );

        shootState
            .Initialized
            .AndThen(value =>
                value == ShootState.Ready || value == ShootState.Preparing
                    ? castUpdate
                    : Stream.None<Void>()
            )
            .Get(_ =>
            {
                var mouse =
                    Input.mousePosition;

                var ray =
                    camera.ScreenPointToRay(mouse);

                if (plane.Raycast(ray, out var distance))
                {
                    var intersection =
                        ray.GetPoint(distance);

                    focus.position =
                        focus.position
                            .WithY(
                                Mathf.Max(
                                    intersection.y,
                                    creature.feet.position.y
                                )
                            );
                }
            });

        // Click to consume a "cut"


        shootState
            .AndThen(value =>
                (value == ShootState.Ready || value == ShootState.Preparing)
                    ? castUpdate
                    : Stream.None<Void>()
            )
            .Filter(_ => cuts.Value > 0 && Input.GetMouseButtonDown(0))
            .Get(_ =>
            {
                shootState.Value =
                    ShootState.AboutToShoot;
            });

        // Preparing

        var animationEvents =
            Query
                .From(creature)
                .Get<AiraAbilityAnimationEvents>();

        var animator =
            Query
                .From(creature, "mesh")
                .Get<Animator>();

        shootState
            .Get(state =>
            {
                animator
                    .SetFloat(
                        "attack_magic_speed",
                        state == ShootState.Preparing ? 0.5f :
                        state == ShootState.Ready ? 0.0f :
                        state == ShootState.Shooting ? 1.5f : 1.0f
                    );
            });

        shootState
            .Filter(state => state == ShootState.Preparing)
            .Filter(_ => cuts.Value > 0)
            .Get(_ =>
            {
                animator
                    .SetTrigger("attack_magic");
            });

        animationEvents
            .magicReady
            .Get(_ =>
            {
                if (shootState.Value == ShootState.Preparing)
                {
                    shootState.Value =
                        ShootState.Ready;
                }
            });

        animationEvents
            .magicCast
            .Get(_ =>
            {
                if (shootState.Value == ShootState.AboutToShoot
                    && cuts.Value > 0)
                {
                    shootState.Value =
                        ShootState.Shooting;

                    cuts.Value--;
                }
            });

        // Shooting

        var shootDuration =
            0.4f;

        var shootDistance =
            75.0f;

        shootState
            .AndThen(value =>
                value == ShootState.Shooting ? castUpdate.Always(Time.time) : Stream.None<float>()
            )
            .Get(shootStartTime =>
            {
                var t =
                    Mathf.Min(
                        1.0f,
                        (Time.time - shootStartTime) / shootDuration
                    );

                magicEnergy.localPosition =
                    new Vector3(0.0f, 0.0f, t * shootDistance);


                if (t >= 1)
                {
                    shootState.Value =
                        ShootState.Preparing;
                }
            });


        Stream.Merge(
            castStart
                .Always(new Void()),
            shootState
                .Filter(value => value == ShootState.Preparing && cuts.Value > 0)
                .Always(new Void())
        )
            .Get(_ =>
            {
                magicEnergy.localPosition =
                    Vector3.zero;
            });


        // Cast end

        var castEnd =
            cuts
                .Filter(value => value == 0)
                .AndThen(Functions.WaitForSeconds<int>(update, 0.8f));

        castEnd
            .Get(_ =>
            {
                battle
                    .CreatureEndsTurn();
            });

        // Show cuts

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

        // Trail & line

        var trail =
            Query
                .From(this, "trail")
                .Get<TrailRenderer>();

        shootState
            .Initialized
            .Get(value =>
            {
                if (value == ShootState.Preparing)
                    trail.Clear();

                // trail.emitting =
                //     value == ShootState.Shooting;
            });

        var line =
            Query
                .From(this, "line")
                .Get();

        shootState
            .Initialized
            .Get(value =>
            {
                line.SetActive(value == ShootState.Ready && cuts.Value > 0);
            });
    }

}
