using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject _unitsInBattleRoot = null;

    Creature[] _unitsInBattle;

    void Start() {
        var unitsInBattle = _unitsInBattle = _unitsInBattleRoot.GetComponentsInChildren<Creature>();

        StartNewTurn();
    }

    int _currentUnitIndex = -1;

    void StartNewTurn() {
        _currentUnitIndex = (_currentUnitIndex + 1) % _unitsInBattle.Length;
        GameState.currentUnit = _unitsInBattle[_currentUnitIndex];

        EventController.TriggerEvent(new StartUnitTurnEvent());
    }

    void OnEnable() {
        EventController.AddListener<EndUnitTurnEvent>(OnUnitTurnEnd);
    }

    void OnDisable() {
        EventController.RemoveListener<EndUnitTurnEvent>(OnUnitTurnEnd);
    }

    void OnUnitTurnEnd(EndUnitTurnEvent e) {
        // Remove dead units from _unitsInBattle array.
        StartNewTurn();
    }
}
