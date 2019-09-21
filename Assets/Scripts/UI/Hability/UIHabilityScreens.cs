using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class UIHabilityScreens : MonoBehaviour
{
    [SerializeField] GameObject _selectHabilityScreen = null;
    [SerializeField] GameObject _castHabilityScreen = null;

    GameObject _currentScreen;

    void OnEnable() {
        EventController.AddListener<TurnStartEvent>(OnTurnStart);
        EventController.AddListener<HabilitySelectEvent>(OnHabilitySelect);
        EventController.AddListener<HabilityCancelEvent>(OnHabilityCancel);
        EventController.AddListener<TurnEndEvent>(OnTurnEnd);
    }

    void OnDisable() {
        EventController.RemoveListener<TurnStartEvent>(OnTurnStart);
        EventController.RemoveListener<HabilitySelectEvent>(OnHabilitySelect);
        EventController.RemoveListener<HabilityCancelEvent>(OnHabilityCancel);
        EventController.RemoveListener<TurnEndEvent>(OnTurnEnd);
    }

    void OnTurnStart(TurnStartEvent evt)
    {
        if (GameState.IsPlayersTurn())
        {
            _selectHabilityScreen.SetActive(true);
        }
        else
        {
            _selectHabilityScreen.SetActive(false);
            _castHabilityScreen.SetActive(false);
        }
    }

    GameObject _instance;

    void OnHabilitySelect(HabilitySelectEvent evt)
    {
        if (!GameState.IsPlayersTurn()) return;

        _selectHabilityScreen.SetActive(false);
        _castHabilityScreen.GetComponentInChildren<FillHabilityDescription>().selectedHability = evt.hability;
        _castHabilityScreen.SetActive(true);

        _instance = Instantiate(evt.hability.controller);
    }

    void OnHabilityCancel(HabilityCancelEvent evt)
    {
        if (!GameState.IsPlayersTurn()) return;

        _selectHabilityScreen.SetActive(true);
        _castHabilityScreen.SetActive(false);

        Destroy(_instance);
    }

    void OnTurnEnd(TurnEndEvent evt)
    {
        Destroy(_instance);
        EventController.TriggerEvent(new TurnEndEvent());
    }
}
