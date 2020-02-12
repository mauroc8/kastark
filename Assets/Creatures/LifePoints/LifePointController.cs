using System;
using UnityEngine;

public enum LifePointState
{
    Idle,
    Belt,
    Dance,
    Dead
};

public static partial class _
{
    public static LifePointState ChangeLifePointAnimation(LifePointState lifePoint, LifePointState animation)
    {
        if (lifePoint == LifePointState.Dead)
            return lifePoint;
        else
            return animation;
    }
}

public class LifePointController : StreamBehaviour
{
    private int _index = -1;
    private Creature _creature;

    [Header("Idle State")]
    public MultiAlphaController alphaController;

    public void Init(int index)
    {
        _index = index;
    }

    protected override void Awake()
    {
        Debug.Assert(_index != -1);

        _creature = GetContext<Creature>();

        var stateChange =
            _creature.StateStream
                .Map(creature => creature.lifePoints[_index])
                .Lazy();

        stateChange
            .Get(state =>
            {
                if (state == LifePointState.Dead)
                {
                    alphaController.FadeOut(0.3f, 2f);
                }
                else
                {
                    alphaController.FadeIn(1.7f, 1f);
                }
            });

        var timeSinceLastChange =
            stateChange
                .StartWith(LifePointState.Idle)
                .Map(_ => Time.time)
                .AndThen(updateStream.Always)
                .Map(time => Time.time - time);

        var dancePosition = new LifePointSpinSettings
        {
            speed = 1.47f,
            radius = 3.55f,
            minHeight = 0.061f,
            maxHeight = 0.924f,
            amountOfTurns = 1.3f,
            oscillationAmount = 0,
            oscillationSpeed = 0
        }.GetPosition(_index, _creature.maxHealth, _creature.Height);

        var beltPosition = new LifePointSpinSettings
        {
            speed = 2,
            radius = 6.13f,
            minHeight = 0.5f,
            maxHeight = 0.5f,
            amountOfTurns = 1,
            oscillationAmount = 0.07f,
            oscillationSpeed = 9
        }.GetPosition(_index, _creature.maxHealth, _creature.Height);

        var idlePosition = new LifePointSpinSettings
        {
            speed = 0.11f,
            radius = 4.6f,
            minHeight = 0.23f,
            maxHeight = 0.23f,
            amountOfTurns = 1f,
            oscillationAmount = 0.06f,
            oscillationSpeed = 0.08f
        }.GetPosition(_index, _creature.maxHealth, _creature.Height);

        Func<float, Vector3> deadPosition = _ => Vector3.up * 6f;

        stateChange
            .StartWith(LifePointState.Idle)
            .Map(state =>
                state == LifePointState.Dance ?
                    dancePosition :
                state == LifePointState.Belt ?
                    beltPosition :
                state == LifePointState.Idle ?
                    idlePosition :
                state == LifePointState.Dead ?
                    deadPosition :
                    deadPosition)
            .MapTuple(position => (transform.localPosition, position))
            .AndThen((changePosition, position) =>
                timeSinceLastChange
                    .Map(time => time / 0.5f)
                    .Map(t => Mathf.Pow(t, 0.4f))
                    .Map(changeAnimation =>
                        Vector3.Lerp(changePosition, position(Time.time), changeAnimation))
            )
            .Get(newPosition =>
            {
                transform.localPosition = newPosition;
            });

        enableStream.Do(() =>
                _isHit = false);
    }

    bool _isHit = false;

    public void Hit()
    {
        if (_isHit) return;
        _isHit = true;

        _creature.LifePointWasHit(_index);
    }

}
