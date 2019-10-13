using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class MonoBehaviourStateMachine : MonoBehaviour {
    protected MonoBehaviour _currentState = null;
    
    protected virtual void SwitchState(MonoBehaviour newState) {
        if (_currentState == newState) return;
        if (_currentState) _currentState.enabled = false;
        if (_currentState = newState) _currentState.enabled = true;
    }
}
