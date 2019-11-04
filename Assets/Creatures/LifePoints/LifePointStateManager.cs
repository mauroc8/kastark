﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class LifePointStateManager : MonoBehaviourStateMachine
{
    [SerializeField] MonoBehaviour _idleState;
    [SerializeField] MonoBehaviour _capsuleSpinState;
    [SerializeField] MonoBehaviour _beltSpinState;

    public void OnTurnStart()
    {
        SwitchState(_idleState);
    }

    public void OnHabilitySelect()
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

    public void OnTurnEnd()
    {
        SwitchState(_idleState);
    }
}
