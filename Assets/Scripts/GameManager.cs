using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject _unitsInBattleRoot = null;
    
    GameObject[] _units;

    void Start() {
        _units = Util.GetGameObjectChildrens(_unitsInBattleRoot);

        StartNewTurn();
    }

    int _currentUnitIndex = -1;

    void StartNewTurn() {
        _currentUnitIndex = (_currentUnitIndex + 1) % _units.Length;

        EventController.TriggerEvent(new StartUnitTurnEvent{ unit = _units[_currentUnitIndex] });
    }

    void OnEnable() {
        EventController.AddListener<EndUnitTurnEvent>(OnUnitTurnEnd);
    }

    void OnDisable() {
        EventController.RemoveListener<EndUnitTurnEvent>(OnUnitTurnEnd);
    }

    void OnUnitTurnEnd(EndUnitTurnEvent e) {
        // Remove dead units from _units array.
        StartNewTurn();
    }
}
