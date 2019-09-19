using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class PlayerTurn : MonoBehaviour
{
    [SerializeField] GameObject[] _screens = {};
    GameObject _currentScreen;

    void SelectScreen(int index)
    {
        if (_currentScreen != null)
        {
            _currentScreen.SetActive(false);
        }

        if (index == -1) return;

        _currentScreen = _screens[index];
        if (_currentScreen != null)
        {
            _currentScreen.SetActive(true);
        }
    }

    void OnEnable() {
        EventController.AddListener<TurnStartEvent>(OnTurnStart);
        EventController.AddListener<HabilitySelectEvent>(OnHabilitySelect);
        EventController.AddListener<HabilityCastEvent>(OnHabilityCast);
    }

    void OnDisable() {
        EventController.RemoveListener<TurnStartEvent>(OnTurnStart);
        EventController.RemoveListener<HabilitySelectEvent>(OnHabilitySelect);
        EventController.RemoveListener<HabilityCastEvent>(OnHabilityCast);
    }

    void OnTurnStart(TurnStartEvent evt)
    {
        SelectScreen(GameState.IsPlayersTurn() ? 0 : -1);
    }

    GameObject _instance;

    void OnHabilitySelect(HabilitySelectEvent evt)
    {
        if (!GameState.IsPlayersTurn()) return;

        Debug.Log($"HabilitySelected:{evt.hability.Name}");
        SelectScreen(1);
        _instance = Instantiate(evt.hability.controller);
        _instance.transform.SetParent(transform, false);
        _instance.SetActive(true);
    }

    void OnHabilityCast(HabilityCastEvent evt)
    {
        if (!GameState.IsPlayersTurn()) return;
        
        Debug.Log($"HabilityCast: {evt.DamageType}\nDamage: {evt.BaseDamage * evt.Effectiveness[0]}");
        
        StartCoroutine(EndTurn());
    }

    WaitForSeconds _waitBeforeDestroy = new WaitForSeconds(1.2f);

    IEnumerator EndTurn()
    {
        yield return _waitBeforeDestroy;
        Destroy(_instance);
        EventController.TriggerEvent(new TurnEndEvent());
    }
}
