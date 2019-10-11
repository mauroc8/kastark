using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePointStateMachine : StateMachine
{
    [Header("Life Point Movement Controller")]
    [SerializeField] LifePointMovementController _lifePointMovementController = null;
    
    [Header("Idle State")]
    [SerializeField] FadeInController _fadeInController = null;
    [SerializeField] FadeOutController _fadeOutController = null;

    [Header("Shield State")]
    [SerializeField] float _spinSpeed = 1;
    [SerializeField] float _minSpinHeight = 0;
    [SerializeField] float _maxSpinHeight = 5;
    [SerializeField] float _spinRadius = 1;

    LifePointIdleState _idleState;
    LifePointShieldState _spinningState;

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

        _spinningState = new LifePointShieldState{
            lifePointMovementController = _lifePointMovementController,
            lifePointPercentage = lifePointPercentage,
            spinSpeed = _spinSpeed,
            minHeight = _minSpinHeight,
            maxHeight = _maxSpinHeight,
            spinRadius = _spinRadius
        };

        SwitchState(_spinningState);
    }
}
