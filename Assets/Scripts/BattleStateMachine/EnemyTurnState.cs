using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class EnemyTurnState : State
{
    public override void InitState() {
        Debug.Log($"Enemy's turn began.");
    }

    public override void UpdateState() {
    }

    public override void ExitState() {
    }
}
