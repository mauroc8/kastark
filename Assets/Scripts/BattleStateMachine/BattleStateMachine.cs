using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class BattleStateMachine : StateMachine
{
    State _playerTurn;
    State _enemyTurn;

    void Start() {
        _playerTurn = new PlayerTurnState();
        _enemyTurn = new EnemyTurnState();

        ChangeState(_playerTurn);
    }

    void Update() {
        UpdateCurrentState();
    }

    void OnEnable() {
        EventController.AddListener<PartyTurnEndEvent>(OnPartyTurnEnd);
    }

    void OnDisable() {
        EventController.RemoveListener<PartyTurnEndEvent>(OnPartyTurnEnd);
    }

    void OnPartyTurnEnd(PartyTurnEndEvent e) {
        var nextTurn = e.teamId == TeamId.PlayerTeam ? _enemyTurn : _playerTurn;

        ChangeState(nextTurn);
    }
}
