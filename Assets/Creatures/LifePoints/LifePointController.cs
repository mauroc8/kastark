using System;
using UnityEngine;
using static Functions;

public enum LifePointState
{
    Idle,
    Belt,
    Dance,
    Shielded,
    Dead
};

public static partial class Functions
{
    public static LifePointState ChangeLifePointAnimation(
        LifePointState lifePoint,
        LifePointState animation)
    {
        if (lifePoint == LifePointState.Dead)
            return lifePoint;
        else
            return animation;
    }
}

// Lifepoints always move spinning. `Spin` is how I call their movement.

delegate SpinCoordinates Spin(float time);

struct SpinCoordinates
{
    public PolarVector2 position;
    public float height;

    public SpinCoordinates(float radius, float polar, float height)
    {
        this.position = new PolarVector2(radius, polar);
        this.height = height;
    }

    public Vector3 ToVector3()
    {
        var v2 = position.ToVector2();

        return new Vector3
        (
            v2.x,
            height,
            v2.y
        );
    }

    public static SpinCoordinates Lerp(SpinCoordinates from, SpinCoordinates to, float amount)
    {
        return new SpinCoordinates
        {
            position = PolarVector2.Lerp(from.position, to.position, amount),
            height = Mathf.Lerp(from.height, to.height, amount)
        };
    }
}

[RequireComponent(typeof(AudioSource))]
public class LifePointController : StreamBehaviour
{
    public MultiAlphaController alphaController;

    private int _index = -1;
    private int _maxIndex = -1;
    private float _creatureHeight = -1;

    private float _percentage => (float)_index / (float)_maxIndex;
    private readonly float PHI = (1 + Mathf.Sqrt(5)) / 2;

    private float _amountAdjustment => (float)_maxIndex / 10.0f;

    public void Init(int index, int maxIndex, float creatureHeight)
    {
        _index = index;
        _maxIndex = maxIndex;
        _creatureHeight = creatureHeight;
    }

    SpinCoordinates danceSpin(float t)
    {
        return new SpinCoordinates
        (
            radius: 5.0f + 2.0f * _amountAdjustment,
            polar: Angle.Turn * (_index % 2 / 2 + _percentage * _amountAdjustment + t / PHI),
            height: (0.06f + _percentage * 0.84f) * _creatureHeight
        );
    }

    SpinCoordinates beltSpin(float t)
    {
        return new SpinCoordinates
        (
            radius: 9.13f + 2.0f * _amountAdjustment,
            polar: _percentage * Angle.Turn * _amountAdjustment + t * 2.4f,
            height:
                (0.5f
                    + 0.17f * Mathf.Sin(_percentage * Angle.Turn * 1.63f + t * 3.3f)
                ) * _creatureHeight
        );
    }

    SpinCoordinates shieldedSpin(float t)
    {
        var growRadius = 3.0f;

        return new SpinCoordinates
        (
            radius:
                Mathf.Sin(_percentage * Angle.Turn + t * Angle.Turn * 0.7f)
                    * (growRadius / 2.0f) + 6.0f + 2.0f * _amountAdjustment,
            polar: _percentage * Angle.Turn + t * 1.8f,
            height:
                (0.5f
                    + 0.17f * Mathf.Sin(_percentage * Angle.Turn * .163f + t * 3.3f)
                ) * _creatureHeight
        );
    }

    SpinCoordinates idleSpin(float t)
    {
        return new SpinCoordinates
        (
            radius: 6.6f + 2.0f * _amountAdjustment,
            polar: _percentage * Angle.Turn + t * 0.1f,
            height:
                0.23f * _creatureHeight
                    + 0.06f * Mathf.Sin(_percentage * Angle.Turn + t * 1.6f)
        );
    }

    SpinCoordinates deadSpin(float t)
    {
        return new SpinCoordinates
        (
            radius: 6.6f + 2.0f * _amountAdjustment,
            polar: _percentage * Angle.Turn + t * 0.1f,
            height: 0
        );
    }

    SpinCoordinates aboutToSpawnSpin(float t)
    {
        return new SpinCoordinates
        (
            radius: 4.6f,
            polar: 0.07f,
            height:
                0.7f * _creatureHeight
        );
    }

    protected override void Awake()
    {
        // > Should have called Init() before SetActive(true).
        Debug.Assert(_index != -1);

        var creature = GetComponentInParent<Creature>();

        var stateChange =
            creature.State
                .Map(state => state.lifePoints[_index])
                .Lazy();

        stateChange
            .WithLastValue(LifePointState.Idle)
            .Get((lastState, state) =>
            {
                if (state == LifePointState.Dead)
                {
                    alphaController.FadeOut(0.3f, 2f);
                }
                else if (lastState == LifePointState.Dead)
                {
                    alphaController.FadeIn(1.7f, 1f);
                }
            });


        stateChange
            .Map(state =>
                state == LifePointState.Dance ?
                    danceSpin :
                state == LifePointState.Belt ?
                    beltSpin :
                state == LifePointState.Idle ?
                    idleSpin :
                state == LifePointState.Dead ?
                    deadSpin :
                state == LifePointState.Shielded ?
                    shieldedSpin :
                    (Spin)idleSpin)
            .WithLastValue(idleSpin)
            .AndThen((lastSpin, spin) =>
            {
                var duration = 0.4f;

                if (lastSpin == deadSpin)
                {
                    // This just makes the spawn transition
                    // different from the dead transition.
                    lastSpin = aboutToSpawnSpin;
                }

                return
                    stateChange
                        .Map(_ => Time.time)
                        .AndThen(update.Always)
                        .Map(time => EaseInOut((Time.time - time) / duration))
                        .Map(amount =>
                        {
                            return SpinCoordinates
                                .Lerp(lastSpin(Time.time), spin(Time.time), amount)
                                .ToVector3();
                        });
            })
            .Get(position =>
            {
                transform.localPosition = position;
            });

        var hitSoundSource =
            GetComponent<AudioSource>();

        stateChange
            .Lazy()
            .Get(state =>
            {
                if (state == LifePointState.Dead)
                {
                    Functions.PlaySwooshSound(hitSoundSource);
                }
            });
    }

    public void Hit()
    {
        GetComponentInParent<Creature>().LifePointWasHit(_index);
    }
}
