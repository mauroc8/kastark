using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    State _currentState;

    protected virtual void Update() {
        UpdateState();
    }

    protected void UpdateState() {
        if (_currentState == null) return;

        _currentState.UpdateState();
    }

    protected void ChangeState(State newState) {
        if (_currentState != null) {
            if (_currentState.GetType() == newState.GetType()) return;
            
            _currentState.ExitState();
        }

        (_currentState = newState).InitState();
    }
}