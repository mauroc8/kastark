using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class GameManager : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] GameSettings _gameSettings = null;
    [SerializeField] GameObject _battleNode = null;

    [Header("References")]
    [SerializeField] GameObject _battleWinNode = null;
    [SerializeField] GameObject _battleLoseNode = null;

    List<CreatureController> _deadCreatures;

    void Awake()
    {
        _gameSettings.Load();
        _gameSettings.Init();
    }

    void Start()
    {
        var allCreatures = _battleNode.GetComponentsInChildren<CreatureController>();
        GameState.creaturesInBattle = new List<CreatureController>(allCreatures);
        _deadCreatures              = new List<CreatureController>();

        foreach (var creatureController in allCreatures)
        {
            creatureController.creature.Init();
        }

        StartCoroutine(StartBattle());
    }

    [SerializeField] float _waitBeforeBattle = 0.1f;

    IEnumerator StartBattle()
    {
        yield return new WaitForSeconds(_waitBeforeBattle);
        EventController.TriggerEvent(new BattleStartEvent{});
        StartNewTurn();
    }

    int _currentUnitIndex = -1;

    void StartNewTurn()
    {
        _currentUnitIndex = (_currentUnitIndex + 1) % GameState.creaturesInBattle.Count;

        var unit = GameState.creaturesInBattle[_currentUnitIndex];

        GameState.actingCreature = unit;
        GameState.actingTeam = GameState.GetTeamOf(unit);

        Debug.Log($"Starting {unit.name}'s turn.");
        
        EventController.TriggerEvent(new TurnStartEvent());
    }

    void OnEnable()
    {
        EventController.AddListener<HabilitySelectEvent>(OnHabilitySelect);
        EventController.AddListener<HabilityCancelEvent>(OnHabilityCancel);
        EventController.AddListener<HabilityCastEvent>(OnHabilityCast);
        EventController.AddListener<TurnEndEvent>(OnTurnEnd);
    }
    void OnDisable()
    {
        EventController.RemoveListener<HabilitySelectEvent>(OnHabilitySelect);
        EventController.RemoveListener<HabilityCancelEvent>(OnHabilityCancel);
        EventController.RemoveListener<HabilityCastEvent>(OnHabilityCast);
        EventController.RemoveListener<TurnEndEvent>(OnTurnEnd);
    }

    GameObject _selectedHabilityInstance = null;
    
    void OnHabilitySelect(HabilitySelectEvent evt)
    {
        GameState.selectedHability = evt.hability;

        _selectedHabilityInstance = Instantiate(evt.hability.controller);
    }

    void OnHabilityCancel(HabilityCancelEvent evt)
    {
        GameState.selectedHability = null;

        Destroy(_selectedHabilityInstance);
        _selectedHabilityInstance = null;
    }

    void OnHabilityCast(HabilityCastEvent evt)
    {
        var targets = evt.Targets;
        var actingCreature = GameState.actingCreature;
        var baseDamage = evt.BaseDamage;
        for (int i = 0; i < targets.Length; i++) {
            actingCreature.Attack(targets[i], baseDamage * evt.Effectiveness[i], evt.DamageType);
        }

        if (targets.Length > 0)
            Debug.Log($"{GameState.selectedHability.name} cast to {targets[0].name} with {baseDamage} base damage and {evt.Effectiveness[0]} effectiveness.");
        else
            Debug.Log($"{GameState.selectedHability.name} cast to noone.");
        
        GameState.selectedHability = null;
    }

    WaitForSeconds _endTurnWait = new WaitForSeconds(0.3f);
    
    void OnTurnEnd(TurnEndEvent evt)
    {
        if (_selectedHabilityInstance != null)
        {
            Destroy(_selectedHabilityInstance);
            _selectedHabilityInstance = null;
        }

        bool leftTeamHasLivingUnit = false;
        bool rightTeamHasLivingUnit = false;

        var removeList = new List<CreatureController>();

        foreach (var creature in GameState.creaturesInBattle)
        {
            if (!creature.IsAlive())
            {
                removeList.Add(creature);
                _deadCreatures.Add(creature);
            } else
            {
                if (GameState.GetTeamOf(creature) == Team.Left)
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
            GameState.creaturesInBattle.Remove(creature);
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
        _battleWinNode.SetActive(true);
    }

    void PlayerLooses()
    {
        _battleLoseNode.SetActive(true);
    }
}
