using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePointStateMachine : StateMachine
{
    [Header("Idle State")]
    [SerializeField] FadeInController _fadeInController = null;
    [SerializeField] FadeOutController _fadeOutController = null;
    
    [Header("Spinning State")]
    [SerializeField] float _spinSpeed = 1;
    [SerializeField] float _minSpinHeight = 0;
    [SerializeField] float _maxSpinHeight = 5;
    [SerializeField] float _spinRadius = 1;
    [SerializeField] float _movementSpeed = 1;
    
    LifePointIdleState _idleState;
    LifePointSpinningState _spinningState;

    void Start()
    {
        var index = transform.GetSiblingIndex();
        var lifePointCount = transform.parent.childCount;
        var lifePointPercentage = (float) index / (float) lifePointCount;

        _idleState = new LifePointIdleState{
            transform = transform,
            fadeInController = _fadeInController,
            fadeOutController = _fadeOutController
        };

        _spinningState = new LifePointSpinningState{
            transform = transform,
            lifePointPercentage = lifePointPercentage,
            spinSpeed = _spinSpeed,
            minHeight = _minSpinHeight,
            maxHeight = _maxSpinHeight,
            spinRadius = _spinRadius,
            movementSpeed = _movementSpeed
        };

        SwitchState(_spinningState);
    }
}
