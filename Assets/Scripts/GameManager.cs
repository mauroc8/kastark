using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class GameManager : MonoBehaviour
{
    // Ahora mismo el GameManager se encarga de dos cosas:
    // De iniciar la batalla, repartir recursos, y también de mantener actualizado el GameState.
    // (Hay que separar la clase.)

    [SerializeField]
    GameSettings _gameSettings = null;

    [SerializeField]
    GameObject _battleNode = null;

    Creature[] _battleParticipants;

    void Awake()
    {
        _gameSettings.Load();
        _gameSettings.Init();
    }

    void Start()
    {
        _battleParticipants = _battleNode.GetComponentsInChildren<Creature>();

        StartCoroutine(StartBattle());
    }

    [SerializeField] float _waitBeforeBattle = 0;

    IEnumerator StartBattle()
    {
        yield return new WaitForSeconds(_waitBeforeBattle);
        EventController.TriggerEvent(new BattleStartEvent{});
        StartNewTurn();
    }

    int _currentUnitIndex = -1;

    void StartNewTurn()
    {
        _currentUnitIndex = (_currentUnitIndex + 1) % _battleParticipants.Length;
        
        var unit = _battleParticipants[_currentUnitIndex];
        GameState.actingCreature = unit;
        GameState.actingTeam = unit.CompareTag("LeftTeam") ? TeamSide.Left : TeamSide.Right;

        EventController.TriggerEvent(new StartCreatureTurnEvent());
    }

    void OnEnable()
    {
        EventController.AddListener<HabilitySelectEvent>(OnHabilitySelect);
        EventController.AddListener<HabilityCastEvent>(OnHabilityCastEnd);
        EventController.AddListener<UnitTurnEndEvent>(OnUnitTurnEnd);
    }
    void OnDisable()
    {
        EventController.RemoveListener<HabilitySelectEvent>(OnHabilitySelect);
        EventController.RemoveListener<HabilityCastEvent>(OnHabilityCastEnd);
        EventController.RemoveListener<UnitTurnEndEvent>(OnUnitTurnEnd);
    }

    void OnHabilitySelect(HabilitySelectEvent evt)
    {
        GameState.selectedHability = evt.hability;
    }

    void OnHabilityCastEnd(HabilityCastEvent evt)
    {
        var targets = evt.Targets;
        var actingCreature = GameState.actingCreature;
        var baseDamage = evt.BaseDamage;
        for (int i = 0; i < targets.Length; i++) {
            actingCreature.Attack(targets[i], baseDamage * evt.Effectiveness[i], evt.DamageType);
        }
    }

    void OnUnitTurnEnd(UnitTurnEndEvent e)
    {
        // Remove dead units from _battleParticipants array.
        // Check if left or right team won.
        StartNewTurn();
    }
}
