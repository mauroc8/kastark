using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameSettings _gameSettings = null;

    [SerializeField]
    GameObject _unitsInBattleRoot = null;

    Creature[] _unitsInBattle;

    void Awake() {
        _gameSettings.Load();
        _gameSettings.Init();
    }

    void Start() {
        _unitsInBattle = _unitsInBattleRoot.GetComponentsInChildren<Creature>();

        StartCoroutine(StartBattle());
    }

    void OnEnable() {
        EventController.AddListener<EndUnitTurnEvent>(OnUnitTurnEnd);
    }

    void OnDisable() {
        EventController.RemoveListener<EndUnitTurnEvent>(OnUnitTurnEnd);
    }

    [SerializeField] float _waitBeforeBattle = 0;

    IEnumerator StartBattle() {
        yield return new WaitForSeconds(_waitBeforeBattle);
        EventController.TriggerEvent(new BattleStartEvent{});
        StartNewTurn();
    }

    int _currentUnitIndex = -1;

    void StartNewTurn() {
        _currentUnitIndex = (_currentUnitIndex + 1) % _unitsInBattle.Length;
        GameState.currentUnit = _unitsInBattle[_currentUnitIndex];

        EventController.TriggerEvent(new StartUnitTurnEvent());
    }

    void OnUnitTurnEnd(EndUnitTurnEvent e) {
        // Remove dead units from _unitsInBattle array.
        // Check if left or right team won.
        StartNewTurn();
    }
}
