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
        Global.creaturesInBattle = new List<CreatureController>(allCreatures);
        _deadCreatures              = new List<CreatureController>();

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
        _currentUnitIndex = (_currentUnitIndex + 1) % Global.creaturesInBattle.Count;

        var creatureController = Global.creaturesInBattle[_currentUnitIndex];

        Global.actingCreature = creatureController;
        Global.actingTeam = Global.GetTeamOf(creatureController);

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
        Global.selectedHability = evt.hability;

        if (Global.IsPlayersTurn())
        {
            _selectedHabilityInstance = Instantiate(evt.hability.controller);
        }
    }

    void OnConsumableSelect(ConsumableSelectEvent evt)
    {
        Global.selectedConsumable = evt.consumable;
    }

    void OnHabilityCancel(HabilityCancelEvent evt)
    {
        Global.selectedHability = null;
        Global.selectedConsumable = null;

        if (_selectedHabilityInstance != null)
        {
            Destroy(_selectedHabilityInstance);
            _selectedHabilityInstance = null;
        }
    }

    void OnHabilityCast(HabilityCastEvent evt)
    {
        if (Global.selectedConsumable != null)
        {
            Global.selectedConsumable.amount--;
        }

        Global.selectedHability = null;
        Global.selectedConsumable = null;
    }

    WaitForSeconds _endTurnWait = new WaitForSeconds(0.7f);

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

        foreach (var creature in Global.creaturesInBattle)
        {
            if (!creature.IsAlive())
            {
                removeList.Add(creature);
                _deadCreatures.Add(creature);
            } else
            {
                if (Global.GetTeamOf(creature) == Team.Left)
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
            Global.creaturesInBattle.Remove(creature);
        }

        if (leftTeamHasLivingUnit && rightTeamHasLivingUnit)
        {
            StartNewTurn();
        } else
        {
            if (leftTeamHasLivingUnit && Global.PlayerTeam == Team.Left ||
                rightTeamHasLivingUnit && Global.PlayerTeam == Team.Right)
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
