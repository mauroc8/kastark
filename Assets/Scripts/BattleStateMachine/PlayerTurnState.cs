using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class PlayerTurnState : State
{
    PlayerTurnBeginEvent _playerTurnBeginEvent = new PlayerTurnBeginEvent();

    public override void InitState() {
        EventController.TriggerEvent(_playerTurnBeginEvent);
    }

    public override void UpdateState() {
    }

    public override void ExitState() {
    }
}
