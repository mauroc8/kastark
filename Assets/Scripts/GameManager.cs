using System.Collections;
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

    [SerializeField] float _waitBeforeBattle = 0;

    IEnumerator StartBattle() {
        yield return new WaitForSeconds(_waitBeforeBattle);
        EventController.TriggerEvent(new BattleStartEvent{});
        StartNewTurn();
    }

    int _currentUnitIndex = -1;

    void StartNewTurn() {
        _currentUnitIndex = (_currentUnitIndex + 1) % _battleParticipants.Length;
        
        var unit = _battleParticipants[_currentUnitIndex];
        GameState.actingCreature = unit;
        GameState.actingTeam = unit.CompareTag("LeftTeam") ? TeamSide.Left : TeamSide.Right;

        EventController.TriggerEvent(new UnitTurnStartEvent());
    }

    void OnEnable() {
        EventController.AddListener<HabilityCastEndEvent>(OnHabilityCastEnd);
        EventController.AddListener<UnitTurnEndEvent>(OnUnitTurnEnd);
    }
    void OnDisable() {
        EventController.RemoveListener<HabilityCastEndEvent>(OnHabilityCastEnd);
        EventController.RemoveListener<UnitTurnEndEvent>(OnUnitTurnEnd);
    }

    void OnHabilityCastEnd(HabilityCastEndEvent e) {
        var targets = e.targets;
        var actingCreature = GameState.actingCreature;
        var baseDamage = e.baseDamage;
        for (int i = 0; i < targets.Length; i++) {
            actingCreature.Attack(targets[i], baseDamage * e.effectiveness[i], e.damageType);
        }
    }

    void OnUnitTurnEnd(UnitTurnEndEvent e) {
        // Remove dead units from _battleParticipants array.
        // Check if left or right team won.
        StartNewTurn();
    }
}
