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

    List<Creature> _battleParticipants;
    List<Creature> _deadCreatures;

    void Awake()
    {
        _gameSettings.Load();
        _gameSettings.Init();
    }

    void Start()
    {
        _battleParticipants = new List<Creature>(_battleNode.GetComponentsInChildren<Creature>());
        _deadCreatures      = new List<Creature>();

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
        _currentUnitIndex = (_currentUnitIndex + 1) % _battleParticipants.Count;

        var unit = _battleParticipants[_currentUnitIndex];

        GameState.actingCreature = unit;
        GameState.actingTeam = GameState.GetTeam(unit);
        
        EventController.TriggerEvent(new TurnStartEvent());
    }

    void OnEnable()
    {
        EventController.AddListener<HabilitySelectEvent>(OnHabilitySelect);
        EventController.AddListener<HabilityCastEvent>(OnHabilityCastEnd);
        EventController.AddListener<TurnEndEvent>(OnTurnEnd);
    }
    void OnDisable()
    {
        EventController.RemoveListener<HabilitySelectEvent>(OnHabilitySelect);
        EventController.RemoveListener<HabilityCastEvent>(OnHabilityCastEnd);
        EventController.RemoveListener<TurnEndEvent>(OnTurnEnd);
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

    WaitForSeconds _endTurnWait = new WaitForSeconds(0.3f);
    
    void OnTurnEnd(TurnEndEvent evt)
    {
        bool leftTeamHasLivingUnit = false;
        bool rightTeamHasLivingUnit = false;

        var removeList = new List<Creature>();

        foreach (var creature in _battleParticipants)
        {
            if (!creature.IsAlive())
            {
                removeList.Add(creature);
                _deadCreatures.Add(creature);
            } else
            {
                if (GameState.GetTeam(creature) == Team.Left)
                {
                    leftTeamHasLivingUnit = true;
                } else
                {
                    rightTeamHasLivingUnit = true;
                }
            }
        }

        foreach (var creature in removeList)
        {
            _battleParticipants.Remove(creature);
        }

        if (leftTeamHasLivingUnit && rightTeamHasLivingUnit)
        {
            StartNewTurn();
        } else
        {
            if (leftTeamHasLivingUnit && GameState.PlayerTeam == Team.Left ||
                rightTeamHasLivingUnit && GameState.PlayerTeam == Team.Right)
            {
                PlayerWins();
            } else
            {
                PlayerLooses();
            }
        }
    }

    void PlayerWins()
    {
        Debug.Log("You WIN!");
    }

    void PlayerLooses()
    {
        Debug.Log("You LOSE!");
    }
}
