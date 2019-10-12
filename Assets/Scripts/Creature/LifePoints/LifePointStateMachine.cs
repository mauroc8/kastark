using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class LifePointStateMachine : StateMachine
{
    [Header("Life Point Movement Controller")]
    [SerializeField] LifePointMovementController _lifePointMovementController = null;
    
    [Header("Idle")]
    [SerializeField] FadeInController _fadeInController = null;
    [SerializeField] FadeOutController _fadeOutController = null;

    [Header("Shield-like spinaround")]
    [SerializeField] float _shieldSpinSpeed = 1;
    [SerializeField] float _shieldMinSpinHeight = 0;
    [SerializeField] float _shieldMaxSpinHeight = 5;
    [SerializeField] float _shieldSpinRadius = 1;
    [SerializeField] float _shieldAmountOfTurns = 12;

    [Header("Belt-like spinaround")]
    [SerializeField] float _beltSpinSpeed = 1;
    [SerializeField] float _beltMinSpinHeight = 0;
    [SerializeField] float _beltMaxSpinHeight = 5;
    [SerializeField] float _beltSpinRadius = 1;
    [SerializeField] float _beltAmountOfTurns = 1;

    LifePointIdleState _idleState;
    LifePointSpinState _shieldSpinState;
    LifePointSpinState _beltSpinState;

    // Set STATES

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

        _shieldSpinState = new LifePointSpinState{
            lifePointMovementController = _lifePointMovementController,
            lifePointPercentage = lifePointPercentage,

            spinSpeed = _shieldSpinSpeed,
            minHeight = _shieldMinSpinHeight,
            maxHeight = _shieldMaxSpinHeight,
            spinRadius = _shieldSpinRadius,
            amountOfTurns = _shieldAmountOfTurns
        };

        _beltSpinState = new LifePointSpinState{
            lifePointMovementController = _lifePointMovementController,
            lifePointPercentage = lifePointPercentage,

            spinSpeed = _beltSpinSpeed,
            minHeight = _beltMinSpinHeight,
            maxHeight = _beltMaxSpinHeight,
            spinRadius = _beltSpinRadius,
            amountOfTurns = _beltAmountOfTurns
        };
    }

    // Change STATES

    void OnEnable()
    {
        EventController.AddListener<TurnStartEvent>(OnTurnStart);
        EventController.AddListener<HabilitySelectEvent>(OnHabilitySelect);
    }

    void OnDisable()
    {
        EventController.RemoveListener<TurnStartEvent>(OnTurnStart);
        EventController.RemoveListener<HabilitySelectEvent>(OnHabilitySelect);
    }

    void OnTurnStart(TurnStartEvent evt)
    {
        SwitchState(_idleState);
    }

    void OnHabilitySelect(HabilitySelectEvent evt)
    {
        if (!Global.IsPlayersTurn() || Global.IsFromActingTeam(gameObject)) return;

        if (Random.Range(0, 100) < 50)
        {
            SwitchState(_shieldSpinState);
        }
        else
        {
            SwitchState(_beltSpinState);
        }
    }
}
