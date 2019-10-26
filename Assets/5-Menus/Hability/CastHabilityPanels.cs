using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class CastHabilityPanels : MonoBehaviour
{
    [SerializeField] GameObject _selectHabilityScreen = null;

    GameObject _currentScreen;

    void OnEnable() {
        EventController.AddListener<TurnStartEvent>(OnTurnStart);
        EventController.AddListener<HabilityCancelEvent>(OnHabilityCancel);
        EventController.AddListener<HabilitySelectEvent>(OnHabilitySelect);
    }

    void OnDisable() {
        EventController.RemoveListener<TurnStartEvent>(OnTurnStart);
        EventController.RemoveListener<HabilityCancelEvent>(OnHabilityCancel);
        EventController.RemoveListener<HabilitySelectEvent>(OnHabilitySelect);
    }

    void OnTurnStart(TurnStartEvent evt)
    {
        _selectHabilityScreen.SetActive(Global.IsPlayersTurn());
    }

    void OnHabilityCancel(HabilityCancelEvent evt)
    {
        _selectHabilityScreen.SetActive(Global.IsPlayersTurn());
    }

    GameObject _instance;

    void OnHabilitySelect(HabilitySelectEvent evt)
    {
        if (!Global.IsPlayersTurn()) return;

        _selectHabilityScreen.SetActive(false);
    }
}
