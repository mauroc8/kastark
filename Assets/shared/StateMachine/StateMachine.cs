using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class StateMachine : MonoBehaviour {

    protected State _currentState;
    
    protected delegate void UpdateState(float dt);
    protected UpdateState _updateState;
    
    protected virtual void SwitchState(State newState) {
        
        if (_currentState != null && _currentState.GetType() == newState.GetType()) {
            return;
        }
        
        _currentState?.ExitState();
        _currentState = newState;
        if (_currentState != null)
        {
            _currentState.InitState();
            _updateState = _currentState.UpdateState;
        }
    }

    protected virtual void Update()
    {
        if (_currentState == null) {
            return;
        }
        _updateState(Time.deltaTime);
    }

    protected bool IsStateRunning(Type state) {
        return (_currentState.GetType() == state);
    }
}