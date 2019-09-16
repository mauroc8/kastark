﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameSettings _gameSettings = null;

    [SerializeField]
    GameObject _battleNode = null;

    Creature[] _battleParticipants;

    void Awake() {
        _gameSettings.Load();
        _gameSettings.Init();
    }

    void Start() {
        _battleParticipants = _battleNode.GetComponentsInChildren<Creature>();

        StartCoroutine(StartBattle());
    }

    void OnEnable() {
        EventController.AddListener<UnitTurnEndEvent>(OnUnitTurnEnd);
    }

    void OnDisable() {
        EventController.RemoveListener<UnitTurnEndEvent>(OnUnitTurnEnd);
    }

    [SerializeField] float _waitBeforeBattle = 0;

    IEnumerator StartBattle() {
        yield return new WaitForSeconds(_waitBeforeBattle);
        EventController.TriggerEvent(new BattleStartEvent{});
        StartNewTurn();
    }

    int _currentUnitIndex = -1;

    void StartNewTurn() {
        _currentUnitIndex = (_currentUnitIndex + 1) % _battleParticipants.Length;
        
        GameState.Instance.actingUnit = _battleParticipants[_currentUnitIndex];

        EventController.TriggerEvent(new UnitTurnStartEvent());
    }

    void OnUnitTurnEnd(UnitTurnEndEvent e) {
        // Remove dead units from _battleParticipants array.
        // Check if left or right team won.
        StartNewTurn();
    }
}
