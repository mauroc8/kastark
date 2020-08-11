using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

public class Shield : Ability
{
    public AudioClip createShield = null;
    public AudioClip hitShield = null;
    public AudioClip destroyShield = null;

    public override bool IsAgressive =>
        false;

    public int amountOfCuts = 5;

    void Awake()
    {
        var battle =
            GetComponentInParent<Battle>();

        var creature =
            GetComponentInParent<Creature>();

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

        castStart
            .AndThen(Functions.WaitForSeconds<bool>(update, 0.3f))
            .Get(_ =>
            {
                creature.shield.Value = amountOfCuts;
            });

        castStart
            .AndThen(Functions.WaitForSeconds<bool>(update, 1.2f))
            .Get(_ =>
            {
                battle
                    .CreatureEndsTurn();
            });

        // --- Shield mesh ---

        var opacity =
            creature
                .shield
                .Initialized
                .Map(value => value > 0 ? 1.0f : 0.0f)
                .AndThen(Functions.LerpStreamOverTime(update, 0.6f));

        var alphaController =
            Query
                .From(this, "shield-mesh")
                .Get<Renderer>();

        var material =
            alphaController.material;

        var maxAlpha =
            material.GetFloat("_Alpha");

        opacity
            .Get(value =>
            {
                material
                    .SetFloat(
                        "_Alpha",
                        Mathf.Pow(value, 10.0f) * maxAlpha
                    );
            });

        var shieldMesh =
            Query
                .From(this, "shield-mesh")
                .Get();

        opacity
            .Initialized
            .Map(t => t != 0)
            .Lazy()
            .Get(shieldMesh.SetActive);

        var collider =
            Query
                .From(shieldMesh)
                .Get<Collider>();

        creature
            .shield
            .Initialized
            .Map(value => value != 0)
            .Lazy()
            .Get(value =>
            {
                collider.enabled = value;
            });

        // --- Hit bar ---

        var hitBar =
            Query
                .From(this, "cut-bar")
                .Get();

        var hits =
            Query
                .From(hitBar)
                .GetAll<AbilityCut>();

        creature
            .shield
            .Initialized
            .Get(value =>
            {
                hitBar.SetActive(value != 0);

                for (int i = 0; i < hits.Length; i++)
                {
                    if (i < value)
                        hits[i].Filled();
                    else
                        hits[i].Empty();
                }
            });


        // Show amount of hits

        for (int i = 0; i < hits.Length; i++)
        {
            hits[i].gameObject.SetActive(i < amountOfCuts);
        }

        // Change screen position

        var hitBarPosition =
            Query
                .From(this, "cut-bar-position")
                .Get<Transform>();

        var hitBarPanel =
            Query
                .From(hitBar, "panel")
                .Get<RectTransform>();

        var camera =
            Camera.main;

        castStart
            .Get(_ =>
            {
                hitBarPanel.position =
                    (camera
                        .WorldToScreenPoint(hitBarPosition.position)
                        + Vector3.up * 45.0f
                    ).WithZ(0);
            });

        // Play audio clips

        var audioSource =
            GetComponent<AudioSource>();

        Action<AudioClip> playSound = clip =>
        {
            if (clip == null)
                return;

            audioSource.clip = clip;
            audioSource.Play();
        };

        creature
            .shield
            .WithLastValue(0)
            .Get((lastShield, currentShield) =>
            {
                if (lastShield > 0 && currentShield == 0)
                {
                    playSound(destroyShield);
                }
                else if (lastShield > currentShield)
                {
                    playSound(hitShield);
                }
                else if (lastShield < currentShield)
                {
                    playSound(createShield);
                }
            });
    }
}