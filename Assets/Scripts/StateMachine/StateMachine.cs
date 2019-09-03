using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    State _currentState = null;

    protected void UpdateCurrentState() {
        if (_currentState == null) return;

        _currentState.UpdateState();
    }

    protected void ChangeState(State newState) {
        if (_currentState == newState) return;

        if (_currentState != null) {    
            _currentState.ExitState();
        }

        _currentState = newState;
        
        if (_currentState != null) {    
            _currentState.InitState();
        }
    }
}