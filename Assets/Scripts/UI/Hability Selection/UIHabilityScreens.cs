using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class UIHabilityScreens : MonoBehaviour
{
    [SerializeField] GameObject _selectHabilityScreen = null;
    [SerializeField] GameObject _castHabilityScreen = null;
    [SerializeField] HabilityDescriptionFiller _habilityDescriptionFiller = null;

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
        _castHabilityScreen.SetActive(false);
    }

    void OnHabilityCancel(HabilityCancelEvent evt)
    {
        _selectHabilityScreen.SetActive(Global.IsPlayersTurn());
        _castHabilityScreen.SetActive(false);
    }

    GameObject _instance;

    void OnHabilitySelect(HabilitySelectEvent evt)
    {
        if (!Global.IsPlayersTurn()) return;

        _selectHabilityScreen.SetActive(false);
        _habilityDescriptionFiller.FillWithHability(evt.hability);
        _castHabilityScreen.SetActive(true);
    }
}
