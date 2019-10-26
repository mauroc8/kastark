using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class GameManager : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] LanguageSettings _gameSettings;
    [SerializeField] GameObject _battleNode;

    List<CreatureController> _creaturesInBattle;

    [Header("References")]
    [SerializeField] GameObject _battleWinNode;
    [SerializeField] GameObject _battleLoseNode;

    void Awake()
    {
        _gameSettings.Load();
        _gameSettings.Init();
    }

    void Start()
    {
        _creaturesInBattle = new List<CreatureController>(
            _battleNode.GetComponentsInChildren<CreatureController>()
        );
        //_creaturesInBattle = _creaturesInBattle;

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
        _currentUnitIndex = (_currentUnitIndex + 1) % _creaturesInBattle.Count;

        var creatureController = _creaturesInBattle[_currentUnitIndex];

        creatureController.StartTurn();

        Global.actingCreature = creatureController;
        Global.actingTeam = Global.GetTeamOf(creatureController);

        EventController.TriggerEvent(new TurnStartEvent{
            actingCreature = creatureController
        });
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

    GameObject _selectedHabilityInstance;

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

        foreach (var creature in _creaturesInBattle)
        {
            if (!creature.IsAlive())
            {
                removeList.Add(creature);
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

        removeList.ForEach(creature => _creaturesInBattle.Remove(creature));

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
