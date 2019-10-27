using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class ChangeLifePointsState : MonoBehaviourStateMachine
{
    [SerializeField] LifePointIdleState _idleState = null;
    [SerializeField] LifePointSpinState _capsuleSpinState = null;
    [SerializeField] LifePointSpinState _beltSpinState = null;

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
        if (Random.Range(0, 100) < 50)
        {
            SwitchState(_capsuleSpinState);
        }
        else
        {
            SwitchState(_beltSpinState);
        }
    }
}
